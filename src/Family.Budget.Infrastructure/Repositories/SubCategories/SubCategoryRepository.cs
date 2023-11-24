namespace Family.Budget.Infrastructure.Repositories.SubCategories;

using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class SubCategoryRepository : QueryHelper<SubCategory>, ISubCategoryRepository
{
    public SubCategoryRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(SubCategory entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(SubCategory entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(SubCategory entity, CancellationToken cancellationToken)
        => Task.FromResult(_dbSet.Remove(entity));

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var ids = new object[] { id };
        var item = await _dbSet.FindAsync(ids, cancellationToken);

        if (item != null)
        {
            _dbSet.Remove(item);
        }
    }

    public async Task<SubCategory?> GetById(Guid id, CancellationToken cancellationToken)
        => await _dbSet
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<SubCategory>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<SubCategory, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower());

        Expression<Func<SubCategory, object>> include = x => x.Category;

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            include,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<SubCategory>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<SubCategory>> GetByName(string name, CancellationToken cancellationToken)
        => Task.FromResult(GetMany(x => x.Name.Equals(name))
            .ToList());

    public Task<SearchOutput<SubCategory>> GetByCategory(SearchInput input, Guid categoryId, CancellationToken cancellationToken)
    {
        Expression<Func<SubCategory, bool>> where = x => x.Category.Id == categoryId;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower()) && x.Category.Id == categoryId;

        Expression<Func<SubCategory, object>> include = x => x.Category;

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            include,
            out var total)
            .Include(x => x.DefaultPaymentMethod)
            .ToList();

        return Task.FromResult(new SearchOutput<SubCategory>(input.Page, input.PerPage, total, items!));
    }


    public async Task<List<SubCategory>> GetByIds(List<Guid> ids, CancellationToken cancellationToken)
        => await _dbSet.Where(x => ids.Contains(x.Id))
            .Include(x => x.Category)
            .ToListAsync(cancellationToken);
}
