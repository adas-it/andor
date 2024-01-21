namespace Family.Budget.Infrastructure.Repositories.FinancialMovements;

using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class FinancialMovementRepository : QueryHelper<FinancialMovement>, IFinancialMovementRepository
{
    public FinancialMovementRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(FinancialMovement entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(FinancialMovement entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(FinancialMovement entity, CancellationToken cancellationToken)
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

    public async Task<FinancialMovement?> GetById(Guid id, CancellationToken cancellationToken)
    => await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<FinancialMovement>> Search(SearchInputMovement input, CancellationToken cancellationToken)
    {
        DateTime startDate = new DateTime(input.Year, input.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(-1).ToUniversalTime();
        DateTime endDate = new DateTime(input.Year, input.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddMilliseconds(-1).ToUniversalTime();

        Expression<Func<FinancialMovement, bool>> where = x =>
            x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false && x.AccountId == (Guid)input.AccountId;

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            where = x => (x.Description ?? "").ToLower().Contains(input.Search.ToLower()) &&
            x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false && x.AccountId == (Guid)input.AccountId;
        }

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .Include(x => x.SubCategory)
            .ThenInclude(x => x.Category)
            .Include(x => x.PaymentMethod)
            .ToList();

        return Task.FromResult(new SearchOutput<FinancialMovement>(input.Page, input.PerPage, total, items!));
    }
    public Task<List<FinancialMovement>> GetAllFinancialMovementsByMonth(
        Guid accountId,
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        DateTime startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(-1).ToUniversalTime();
        DateTime endDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddMilliseconds(-1).ToUniversalTime();

        var query = _dbSet.AsNoTracking();

        query = query.Where(x => x.AccountId == accountId);
        query = query.Where(x => x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false);

        return query.ToListAsync(cancellationToken);
    }

    public Task<DateTime?> GetFirstMovement(Guid accountId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_dbSet.OrderBy(x => x.Date).First(x => x.AccountId == accountId && x.IsDeleted == false)?.Date);
    }

    public Task<DateTime?> GetLastMovement(Guid accountId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_dbSet.OrderByDescending(x => x.Date).First(x => x.AccountId == accountId && x.IsDeleted == false)?.Date);
    }
}
