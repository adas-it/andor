namespace Family.Budget.Infrastructure.Repositories.CashFlows;

using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class CashFlowRepository : ICashFlowRepository
{
    protected readonly DbSet<CashFlow> _dbSet;

    public CashFlowRepository(PrincipalContext context)
    {
        _dbSet = context.Set<CashFlow>();
    }

    public async Task Insert(CashFlow entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(CashFlow entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(CashFlow entity, CancellationToken cancellationToken)
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

    public Task<CashFlow?> GetByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken)
        => _dbSet.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Year == year && x.Month == month, cancellationToken);
    public Task<CashFlow?> GetCurrentOrPreviousCashFlowByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken)
        => _dbSet.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefaultAsync(x => x.AccountId == accountId &&
        ((x.Year == year && x.Month <= month) ||
        (x.Year < year)), cancellationToken);

    public Task<CashFlow?> GetPreviousCashFlowByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken)
        => _dbSet.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefaultAsync(x => x.AccountId == accountId && 
        ((x.Year == year && x.Month < month) || 
        (x.Year < year)), cancellationToken);

    public Task<CashFlow?> GetNextCashFlowByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken)
        => _dbSet.OrderBy(x => x.Year).ThenBy(x => x.Month).FirstOrDefaultAsync(x => x.AccountId == accountId &&
        ((x.Year == year && x.Month > month) ||
        (x.Year > year)), cancellationToken);
}
