using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesCashFlowRepository :
    IResearchableRepository<CashFlow, CashFlowId, SearchInput>
{
}
