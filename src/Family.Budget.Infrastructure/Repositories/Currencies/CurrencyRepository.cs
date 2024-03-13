namespace Family.Budget.Infrastructure.Repositories.Currencies;

using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class CurrencyRepository : QueryHelper<Currency>, ICurrencyRepository
{
    public CurrencyRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(Currency entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(Currency entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(Currency entity, CancellationToken cancellationToken)
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

    public async Task<Currency?> GetById(Guid id, CancellationToken cancellationToken)
        => await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Currency>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Currency, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower());

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Currency>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<Currency>> GetByName(string name, CancellationToken cancellationToken)
        => Task.FromResult(GetMany(x => x.Name.Equals(name))
            .ToList());
}
