using Andor.Accounts.Contracts;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Interfaces;

public interface IAccountQueriesService
{
    Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken);

    Task<ApplicationResult<ListAccountOutput>> GetListAsync(SearchInput input, CancellationToken cancellationToken);
}
