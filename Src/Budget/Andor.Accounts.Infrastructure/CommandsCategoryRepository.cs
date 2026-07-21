using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.Categories.Repositories;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsCategoryRepository(AccountsContext context) : ICommandsCategoryRepository
{
    protected readonly DbSet<Category> DbSet = context.Set<Category>();

    public Task<Category?> GetByIdAsync(CategoryId id, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<Category>> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Category> entities = DbSet.Where(x => x.Owner == null).ToList();

        return Task.FromResult(entities);
    }

    public async Task PersistAsync(Category entity, CancellationToken cancellationToken)
    {
        context.Upsert<Category, CategoryId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
