using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsCashFlowRepository(PrincipalContext context) :
    CommandsBaseRepository<CashFlow, CashFlowId>(context),
    ICommandsCashFlowRepository
{
    public async Task<CashFlow?> GetCurrentOrNextCashFlowAsync(AccountId accountId, Year year, Month month, CancellationToken cancellationToken)
        => await _dbSet
        .OrderBy(x => x.Year)
        .ThenBy(x => x.Month)
        .FirstOrDefaultAsync(x => x.AccountId == accountId &&
            ((x.Year == year && x.Month >= month) || (x.Year > year)), cancellationToken);

    public async Task<CashFlow?> GetCurrentOrPreviousCashFlowAsync(AccountId accountId, Year year, Month month, CancellationToken cancellationToken)
        => await _dbSet
        .OrderByDescending(x => x.Year)
        .ThenByDescending(x => x.Month)
        .FirstOrDefaultAsync(x => x.AccountId == accountId &&
            ((x.Year == year && x.Month <= month) || (x.Year < year)), cancellationToken);
}
