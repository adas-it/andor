using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application;

public class AccountQueriesService(IAccountQueriesRepository accountQueriesRepository, ICurrentUserService currentUser) : IAccountQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken)
    {
        var UserId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.GetByIdAsync(id, cancellationToken);

        var resultAccount = result?.ToAccountOutput();

        if (result is not null && result.Members.Any(x => x.UserId == UserId))
        {

            return ApplicationResult<AccountOutput>.Success(Data: resultAccount);
        }

        return ApplicationResult<AccountOutput>.Failure();
    }

    public async Task<ApplicationResult<ListAccountOutput>> GetListAsync(SearchInput input, CancellationToken cancellationToken)
    {
        var UserId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.SearchAsync(input, cancellationToken);

        var resultAccount = new ListAccountOutput(result.CurrentPage, result.PerPage, result.Total,
            result.Items.Where(x => x.Members.Any(m => m.UserId == UserId)).Select(x => x.ToAccountOutput()).ToList());

        return ApplicationResult<ListAccountOutput>.Success(Data: resultAccount);
    }
}
