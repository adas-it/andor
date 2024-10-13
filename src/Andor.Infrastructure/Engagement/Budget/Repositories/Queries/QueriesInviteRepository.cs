using Andor.Application.Common.Interfaces;
using Andor.Domain.Engagement.Budget.Accounts.Invites;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Queries;

public class QueriesInviteRepository :
        QueryHelper<Invite, InviteId>, IQueriesInviteRepository
{
    public QueriesInviteRepository(PrincipalContext context,
        ICurrentUserService _currentUserService) : base(context)
    {
        loggedUserFilter = x => x.Account.Users.Any(z => z.UserId == _currentUserService.User.UserId);
    }

    public Task<SearchOutput<Invite>> SearchAsync(SearchInputInvite input, CancellationToken cancellationToken)
    {
        Expression<Func<Invite, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Email.Equals(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Invite>(input.Page, input.PerPage, total, items!));
    }
}
