using Andor.Domain.Communications.Repositories;
using Andor.Domain.Communications.Users;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Communication.Repositories.Users;

public class QueriesRecipientRepository(PrincipalContext context) :
    QueryHelper<Recipient, RecipientId>(context),
    IQueriesRecipientRepository
{
    public Task<SearchOutput<Recipient>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Recipient, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Recipient>(input.Page, input.PerPage, total, items!));
    }
}

