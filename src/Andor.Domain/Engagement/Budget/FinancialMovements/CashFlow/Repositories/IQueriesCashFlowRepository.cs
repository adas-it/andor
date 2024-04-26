using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesCashFlowRepository :
    IResearchableRepository<CashFlow, CashFlowId, SearchInput>
{
    Task<CashFlow?> GetByAccountIdAsync(Year year, Month month, CancellationToken cancellationToken);
    Task<CashFlow?> GetPreviousCashFlowByAccountIdAsync(Year year, Month month, CancellationToken cancellationToken);
    Task<CashFlow?> GetNextCashFlowByAccountIdAsync(Year year, Month month, CancellationToken cancellationToken);
    Task<CashFlow?> GetCurrentOrPreviousCashFlowByAccountIdAsync(AccountId accountId, Year year, Month month, CancellationToken cancellationToken);
}
