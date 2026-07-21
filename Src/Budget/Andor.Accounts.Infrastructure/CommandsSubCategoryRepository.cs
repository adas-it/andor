using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.SubCategories.Repositories;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsSubCategoryRepository(AccountsContext context) : ICommandsSubCategoryRepository
{
    protected readonly DbSet<SubCategory> DbSet = context.Set<SubCategory>();

    public Task<SubCategory?> GetByIdAsync(SubCategoryId id, CancellationToken cancellationToken)
    {
        var entity = DbSet
            .Include(x => x.Category)
            .Include(x => x.DefaultPaymentMethod)
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<SubCategory>> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<SubCategory> entities = DbSet
            .Include(x => x.Category)
            .Include(x => x.DefaultPaymentMethod)
            .Where(x => x.Owner == null)
            .ToList();

        return Task.FromResult(entities);
    }

    public async Task PersistAsync(SubCategory entity, CancellationToken cancellationToken)
    {
        context.Upsert<SubCategory, SubCategoryId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
