using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.Invites;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
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

    public async Task PersistAsync(Account entity, CancellationToken cancellationToken)
    {
        var exists = await DbSet.AsNoTracking().AnyAsync(x => x.Id == entity.Id, cancellationToken);

        if (!exists)
        {
            DbSet.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            return;
        }

        // Account is loaded/mutated in a different DbContext instance than the one used to
        // persist it (fresh scope per actor command), so the child collections (join entities
        // with client-assigned, non-DB-generated composite keys) arrive detached. A blind
        // context.Upsert/Update on the root would walk the whole graph and mark every reachable
        // entity as Modified (EF can't tell "new" from "existing" once the key is already set),
        // which turns newly-added rows (e.g. AccountCategory) into no-op UPDATEs that silently
        // affect zero rows. Reconcile each collection explicitly instead.
        context.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        await ReconcileChildStatesAsync(
            entity.Categories,
            x => x.CategoryId,
            context.Set<AccountCategory>().Where(x => x.AccountId == entity.Id).Select(x => x.CategoryId),
            cancellationToken);

        await ReconcileChildStatesAsync(
            entity.SubCategories,
            x => x.SubCategoryId,
            context.Set<AccountSubCategory>().Where(x => x.AccountId == entity.Id).Select(x => x.SubCategoryId),
            cancellationToken);

        await ReconcileChildStatesAsync(
            entity.PaymentMethods,
            x => x.PaymentMethodId,
            context.Set<AccountPaymentMethod>().Where(x => x.AccountId == entity.Id).Select(x => x.PaymentMethodId),
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

        await context.SaveChangesAsync(cancellationToken);
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
            context.Entry(child).State = existingKeys.Contains(keySelector(child))
                ? EntityState.Unchanged
                : EntityState.Added;
        }
    }
}
