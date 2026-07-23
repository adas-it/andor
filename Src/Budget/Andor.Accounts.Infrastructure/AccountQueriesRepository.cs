using System.Linq.Expressions;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Foundation.Infrastructure;

namespace Andor.Accounts.Infrastructure;

public class AccountQueriesRepository :
        QueryHelper<Account, AccountId>, IAccountQueriesRepository
{
    public AccountQueriesRepository(AccountsContext context,
    ICurrentUserService _currentUserService) : base(context)
    {
        loggedUserFilter = x => x.Members.Any(z => z.UserId == _currentUserService.GetCurrentUser().UserId);
    }

    public Task<SearchOutput<Account>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Account, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => ((string)x.Name).Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Account>(input.Page, input.PerPage, total, items!));
    }

}
