using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.FinancialMovements.Repositories;

public interface IFinancialMovementQueriesRepository :
    ISearchableRepository<FinancialMovement, FinancialMovementId, SearchInput>
{
    Task<List<FinancialMovement>> GetAllFinancialMovementsByMonth(AccountId accountId, Month month, Year year, CancellationToken cancellationToken);
}
