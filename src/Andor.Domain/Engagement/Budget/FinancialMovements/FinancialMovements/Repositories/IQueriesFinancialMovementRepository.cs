using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesFinancialMovementRepository
{

    Task<FinancialMovement?> GetByIdAsync(FinancialMovementId id, CancellationToken cancellationToken);

    Task<SearchOutput<FinancialMovementOutput>> SearchOutputAsync(SearchInputMovement input, CancellationToken cancellationToken);

    Task<List<FinancialMovementOutput>> GetAllFinancialMovementsByMonth(
        AccountId accountId,
        Year year,
        Month month,
        CancellationToken cancellationToken);

    Task<DateTime?> GetFirstMovement(AccountId accountId,
        CancellationToken cancellationToken);

    Task<DateTime?> GetLastMovement(AccountId accountId,
        CancellationToken cancellationToken);
}
