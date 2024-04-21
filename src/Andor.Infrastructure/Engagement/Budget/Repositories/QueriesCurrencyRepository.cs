using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class QueriesCurrencyRepository(PrincipalContext context) :
    QueryHelper<Currency, CurrencyId>(context),
    IQueriesCurrencyRepository
{
    public Task<SearchOutput<Currency>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Currency, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Currency>(input.Page, input.PerPage, total, items!));
    }
}
