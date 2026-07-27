using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsCashFlowRepository(AccountsContext context) : ICommandsCashFlowRepository
{
    protected readonly DbSet<CashFlow> DbSet = context.Set<CashFlow>();

    public Task<CashFlow?> GetByIdAsync(CashFlowId id, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public Task<CashFlow?> GetByAccountAndPeriodAsync(AccountId accountId, int periodKey, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.AccountId == accountId && x.PeriodKey == periodKey);

        return Task.FromResult(entity);
    }

    public Task<CashFlow?> GetLatestBeforeAsync(AccountId accountId, int periodKey, CancellationToken cancellationToken)
    {
        var entity = DbSet
            .Where(x => x.AccountId == accountId && x.PeriodKey < periodKey)
            .OrderByDescending(x => x.PeriodKey)
            .FirstOrDefault();

        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<CashFlow>> GetAfterAsync(AccountId accountId, int periodKey, CancellationToken cancellationToken)
    {
        IReadOnlyList<CashFlow> entities = DbSet
            .Where(x => x.AccountId == accountId && x.PeriodKey > periodKey)
            .OrderBy(x => x.PeriodKey)
            .ToList();

        return Task.FromResult(entities);
    }

    public async Task PersistAsync(CashFlow entity, CancellationToken cancellationToken)
    {
        context.Upsert<CashFlow, CashFlowId>(entity);

        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<DateTime?> GetLatestPeriodDateAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var latestCashFlow = await context.Set<FinancialMovement>()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);
        return latestCashFlow?.Date;
    }

    public async Task<DateTime?> GetFirstMovementDateAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var firstCashFlow = await context.Set<FinancialMovement>()
            .Where(x => x.AccountId == accountId)
            .OrderBy(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);
        return firstCashFlow?.Date;
    }
}
