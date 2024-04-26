using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesFinancialMovementRepository :
    IResearchableRepository<FinancialMovement, FinancialMovementId, SearchInput>
{
    Task<List<FinancialMovement>> GetAllFinancialMovementsByMonth(
        AccountId accountId,
        Year year,
        Month month,
        CancellationToken cancellationToken);

    Task<DateTime?> GetFirstMovement(AccountId accountId,
        CancellationToken cancellationToken);

    Task<DateTime?> GetLastMovement(AccountId accountId,
        CancellationToken cancellationToken);
}
