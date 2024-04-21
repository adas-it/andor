using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesFinancialMovementRepository :
    IResearchableRepository<FinancialMovement, FinancialMovementId, SearchInput>
{
}
