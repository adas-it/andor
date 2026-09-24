using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Domain.Invites;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsAccountRepository(AccountsContext context) : ICommandsAccountRepository
{
    protected readonly DbSet<Account> DbSet = context.Set<Account>();

    public Task<Account?> GetByIdAsync(AccountId id, CancellationToken cancellationToken)
    {
        var entity = DbSet
            .Include(x => x.Categories)
            .Include(x => x.SubCategories)
            .Include(x => x.PaymentMethods)
            .Include(x => x.Members)
            .Include(x => x.Invites)
            .Include(x => x.Currency)
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult<Account?>(entity);
    }

    public Task<FinancialMovement?> GetFinancialMovementByIdAsync(FinancialMovementId id, CancellationToken cancellationToken)
    {
        // AsNoTracking: FinancialMovement.Validate() requires Account (and Edit() requires
        // SubCategory/PaymentMethod) to be non-null, so the Include below can't just be dropped —
        // but loading it as a *tracked* query used to eagerly track a second, freshly-loaded
        // Account instance in this context. PersistAsync's TrackOrMergeState would then find that
        // instance already tracked under the same key as the actor's long-lived _account and
        // silently merge into it instead of attaching _account itself — dropping whatever domain
        // events _account had just raised (e.g. RemoveFinancialMovement's
        // AccountFinancialMovementRemovedDomainEvent never reached the Outbox). Loading untracked
        // keeps the navigations populated for domain validation while leaving PersistAsync free to
        // attach the real, event-carrying instances explicitly.
        var entity = context.Set<FinancialMovement>()
            .Include(x => x.SubCategory).ThenInclude(x => x.Category)
            .Include(x => x.PaymentMethod)
            .Include(x => x.Account)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id.Equals(id));

        return Task.FromResult(entity);
    }

    public async Task UpsertFinancialMovement(FinancialMovement entity, CancellationToken cancellationToken)
    {
        // SubCategory/PaymentMethod/Account (and everything reachable from them, e.g.
        // SubCategory.Category) are loaded in a different DbContext instance (actor's Loading
        // scope) than this one, so they arrive detached even though they already exist in the
        // database, and may share a row with something already tracked here. Reconciling state
        // by hand still isn't enough on its own: EF's automatic change detection walks every
        // navigation property of every tracked entity and tries to auto-attach whatever it finds
        // there too, which re-triggers the same "already tracked" conflict for entities we never
        // touched directly (e.g. SubCategory.Category). WithManualStateManagement suspends that
        // so only the explicit Entry().State assignments below take effect.
        using var manualStateManagement = WithManualStateManagement();

        TrackOrMergeState(entity.SubCategory, EntityState.Unchanged);
        TrackOrMergeState(entity.PaymentMethod, EntityState.Unchanged);
        //TrackOrMergeState(entity.Account, EntityState.Unchanged);

        // Don't use context.Upsert/Update here: it cascades into the same reachable navigations
        // above (SubCategory/PaymentMethod/Account) and would try to re-attach whichever instance
        // TrackOrMergeState above didn't end up tracking (e.g. because a different-but-same-key
        // instance from a sibling movement upserted moments earlier is already tracked), throwing the
        // same "already tracked" conflict. Mark this entity's own entry directly instead; its
        // SubCategoryId/PaymentMethodId/AccountId scalar FKs are already set, so the related
        // navigations don't need to be walked for the FK columns to persist correctly.
        var exists = await context.Set<FinancialMovement>().AsNoTracking()
            .AnyAsync(x => x.Id.Equals(entity.Id), cancellationToken);

        context.Entry(entity).State = exists ? EntityState.Modified : EntityState.Added;
    }

    /// <summary>
    /// Tracks <paramref name="entity"/> under the given state, unless a *different* instance for
    /// the same row is already tracked in this context — e.g. this FinancialMovement's own
    /// SubCategory/Account (Include-loaded alongside it) versus the SubCategory/Account instance
    /// handed in from a separate load (the actor's Loading scope, or a sibling entity upserted
    /// moments earlier in the same persist operation). EF's identity map only allows one tracked
    /// instance per key, so when that happens, fold this instance's current values into the one
    /// already tracked instead of attaching a duplicate (which would throw "already tracked").
    /// </summary>
    private void TrackOrMergeState<TEntity>(TEntity entity, EntityState state)
        where TEntity : class
    {
        var entry = context.Entry(entity);

        if (entry.State != EntityState.Detached)
        {
            entry.State = state;
            return;
        }

        var key = entry.Metadata.FindPrimaryKey()!;
        var keyValues = key.Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();

        var existing = context.ChangeTracker.Entries<TEntity>().FirstOrDefault(e =>
            !ReferenceEquals(e.Entity, entity) &&
            key.Properties.Select(p => e.Property(p.Name).CurrentValue).SequenceEqual(keyValues));

        if (existing != null)
        {
            existing.CurrentValues.SetValues(entity);

            // Only an explicit "this changed" request should override the existing tracking's
            // state; a mere Unchanged request (the FK-navigation case above) shouldn't downgrade
            // an existing Added/Modified entry back to Unchanged.
            if (state != EntityState.Unchanged)
                existing.State = state;

            return;
        }

        entry.State = state;
    }

    public async Task PersistAsync(Account entity, CancellationToken cancellationToken)
    {
        var exists = await DbSet.AsNoTracking().AnyAsync(x => x.Id == entity.Id, cancellationToken);

        if (!exists)
        {
            _ = DbSet.Add(entity);
            _ = await context.SaveChangesAsync(cancellationToken);
            return;
        }

        // Account is loaded/mutated in a different DbContext instance than the one used to
        // persist it (fresh scope per actor command), so the child collections (join entities
        // with client-assigned, non-DB-generated composite keys) arrive detached. A blind
        // context.Upsert/Update on the root would walk the whole graph and mark every reachable
        // entity as Modified (EF can't tell "new" from "existing" once the key is already set),
        // which turns newly-added rows (e.g. AccountCategory) into no-op UPDATEs that silently
        // affect zero rows. Reconcile each collection explicitly instead.
        //
        // Mark the root's own entry directly rather than context.Attach(entity): Attach cascades
        // into every reachable navigation, including AccountSubCategory.SubCategory/AccountPaymentMethod.PaymentMethod
        // master rows, which may already be tracked under a different instance in this context
        // (e.g. a financial movement upserted earlier in the same persist operation). See
        // WithManualStateManagement for why that's also not enough on its own.
        //
        // Use TrackOrMergeState rather than a direct Entry().State assignment: this same context
        // could in principle already be tracking a *different* Account instance for this row —
        // attaching this one directly would throw "already tracked". (GetFinancialMovementByIdAsync
        // loads its Account Include with AsNoTracking so this doesn't happen on the edit/delete-movement
        // paths; see the comment there.)
        using var manualStateManagement = WithManualStateManagement();

        TrackOrMergeState(entity, EntityState.Modified);

        await ReconcileChildStatesAsync(
            entity.Categories,
            x => x.CategoryId,
            context.Set<AccountCategory>().Where(x => x.AccountId == entity.Id).Select(x => x.CategoryId),
            cancellationToken);

        // The join row above (AccountCategory) is one thing; for a *custom* category (as opposed
        // to attaching an already-seeded template) the master Category row itself is also new and
        // needs its own Added state — TrackOrMergeState only ever touches the entity passed to it,
        // so without this the join row would insert a CategoryId that never got a matching Category
        // row, and the category would silently vanish from Includes on the next read.
        var categoryIds = entity.Categories.Select(x => x.CategoryId).ToList();
        await ReconcileChildStatesAsync(
            entity.Categories.Select(x => x.Category),
            x => x.Id,
            context.Set<Category>().Where(x => categoryIds.Contains(x.Id)).Select(x => x.Id),
            cancellationToken);

        await ReconcileChildStatesAsync(
            entity.SubCategories,
            x => x.SubCategoryId,
            context.Set<AccountSubCategory>().Where(x => x.AccountId == entity.Id).Select(x => x.SubCategoryId),
            cancellationToken);

        var subCategoryIds = entity.SubCategories.Select(x => x.SubCategoryId).ToList();
        await ReconcileChildStatesAsync(
            entity.SubCategories.Select(x => x.SubCategory),
            x => x.Id,
            context.Set<SubCategory>().Where(x => subCategoryIds.Contains(x.Id)).Select(x => x.Id),
            cancellationToken);

        await ReconcileChildStatesAsync(
            entity.PaymentMethods,
            x => x.PaymentMethodId,
            context.Set<AccountPaymentMethod>().Where(x => x.AccountId == entity.Id).Select(x => x.PaymentMethodId),
            cancellationToken);

        var paymentMethodIds = entity.PaymentMethods.Select(x => x.PaymentMethodId).ToList();
        await ReconcileChildStatesAsync(
            entity.PaymentMethods.Select(x => x.PaymentMethod),
            x => x.Id,
            context.Set<PaymentMethod>().Where(x => paymentMethodIds.Contains(x.Id)).Select(x => x.Id),
            cancellationToken);

        await ReconcileChildStatesAsync(
            entity.Members,
            x => x.UserId,
            context.Set<AccountUser>().Where(x => x.AccountId == entity.Id).Select(x => x.UserId),
            cancellationToken);

        await ReconcileChildStatesAsync(
            entity.Invites,
            x => x.Id,
            context.Set<Invite>().Where(x => x.AccountId == entity.Id).Select(x => x.Id),
            cancellationToken);

        _ = await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Suspends EF's automatic change detection for the returned scope's lifetime. Every entity
    /// touched by UpsertFinancialMovement/PersistAsync gets an explicit Entry().State assignment,
    /// so automatic detection isn't needed for correctness — and it's actively harmful here: EF
    /// runs it lazily (on the next ChangeTracker.Entries()/SaveChanges() call) and, as part of it,
    /// walks every navigation property of every tracked entity and auto-attaches whatever it
    /// finds, which throws the same "already tracked under a different instance" conflict for
    /// entities never explicitly touched (e.g. a tracked SubCategory's own Category navigation
    /// pointing at yet another detached, different-context instance of an already-tracked row).
    /// SaveChangesAsync still honors this flag, so it's kept disabled through the trailing save.
    /// </summary>
    private IDisposable WithManualStateManagement()
    {
        var previous = context.ChangeTracker.AutoDetectChangesEnabled;
        context.ChangeTracker.AutoDetectChangesEnabled = false;

        return new Restorer(() => context.ChangeTracker.AutoDetectChangesEnabled = previous);
    }

    private sealed class Restorer(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }

    private async Task ReconcileChildStatesAsync<TChild, TKey>(
        IEnumerable<TChild> children,
        Func<TChild, TKey> keySelector,
        IQueryable<TKey> existingKeysQuery,
        CancellationToken cancellationToken)
        where TChild : class
    {
        var childList = children.ToList();

        if (childList.Count == 0)
            return;

        var existingKeys = (await existingKeysQuery.ToListAsync(cancellationToken)).ToHashSet();

        foreach (var child in childList)
        {
            // TrackOrMergeState, not a direct Entry().State assignment: Account (and its
            // AutoInclude-configured navigations) may already have brought a *different* instance
            // of this same join row into this context — e.g. via FinancialMovement.Account,
            // Include-loaded moments earlier in the same operation — which would otherwise throw
            // "already tracked" here exactly as it does for the FK-navigation entities above.
            TrackOrMergeState(child, existingKeys.Contains(keySelector(child))
                ? EntityState.Unchanged
                : EntityState.Added);
        }
    }
}
