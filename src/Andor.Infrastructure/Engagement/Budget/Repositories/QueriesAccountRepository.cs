using Andor.Application.Common.Models.Authorizations;
using Andor.Domain.Engagement.Budget.Entities.Accounts;
using Andor.Domain.Engagement.Budget.Entities.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class QueriesAccountRepository(PrincipalContext context,
        ICurrentUserService _currentUserService) :
        QueryHelper<Account, AccountId>(context), IQueriesAccountRepository
{
    public override Task<Account?> GetByIdAsync(AccountId id, CancellationToken cancellationToken)
        => Task.FromResult(
            _dbSet.AsNoTracking()
            .FirstOrDefault(x => x.Id == id && x.Users.Any(z => z.UserId == _currentUserService.User.UserId)));

    public Task<SearchOutput<Account>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Account, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

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
