using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.FinancialMovements.Response;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public interface IAccountFinancialMovementsQueriesService
{
    Task<ApplicationResult<ListFinancialMovementsOutput?>> GetByAccountIdAndFinancialMovementAsync(ListFinancialMovementsQuery query, CancellationToken cancellationToken);
    Task<ApplicationResult<FinancialMovementOutput?>> GetByAccountIdAndFinancialMovementIdAsync(AccountId accountId, FinancialMovementId financialMovementId, CancellationToken cancellationToken);
}
