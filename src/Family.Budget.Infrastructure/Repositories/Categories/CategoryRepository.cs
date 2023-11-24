namespace Family.Budget.Infrastructure.Repositories.Categories;

using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Categories.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class CategoryRepository : QueryHelper<Category>, ICategoryRepository
{
    public CategoryRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(Category entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(Category entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(Category entity, CancellationToken cancellationToken)
        => Task.FromResult(_dbSet.Remove(entity));

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var ids = new object[] { id };
        var entity = await _dbSet.FindAsync(ids, cancellationToken);

        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public Task<Category?> GetById(Guid id, CancellationToken cancellationToken)
        => _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Category>> Search(SearchInputCategory input, CancellationToken cancellationToken)
    {
        Expression<Func<Category, bool>> where = x => x.Type.Equals(input.Type);

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower()) && x.Type.Equals(input.Type);

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Category>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<Category>> GetByName(string name, CancellationToken cancellationToken)
        => Task.FromResult(GetMany(x => x.Name.Equals(name)).ToList());
}
