using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Interfaces;

public interface IAccountQueriesService
{
    Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken);

    Task<ApplicationResult<ListAccountOutput>> GetListAsync(SearchInput input, CancellationToken cancellationToken);

    Task<ApplicationResult<CashFlowOutput>> GetCashFlowAsync(AccountId accountId, Month month, Year year,
        CancellationToken cancellationToken);

    Task<ApplicationResult<List<FinancialSummariesOutput>>> GetFinancialSummaryAsync(AccountId accountId, Month month, Year year,
        CancellationToken cancellationToken);

    Task<ApplicationResult<List<CategorySummariesOutput>>> GetCategorySummaryAsync(AccountId accountId, Month month, Year year,
        CancellationToken cancellationToken);

}
