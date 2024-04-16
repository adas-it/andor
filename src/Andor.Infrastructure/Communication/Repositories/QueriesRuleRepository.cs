using Andor.Domain.Communications;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Communication.Repositories;

public class QueriesRuleRepository(PrincipalContext context) :
    QueryHelper<Rule, RuleId>(context),
    IQueriesRuleRepository
{
    public Task<SearchOutput<Rule>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Rule, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Rule>(input.Page, input.PerPage, total, items!));
    }
}

