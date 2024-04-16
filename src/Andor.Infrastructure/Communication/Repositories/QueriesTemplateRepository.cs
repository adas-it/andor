using Andor.Domain.Communications;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Communication.Repositories;

public class QueriesTemplateRepository(PrincipalContext context) :
    QueryHelper<Template, TemplateId>(context),
    IQueriesTemplateRepository
{
    public Task<SearchOutput<Template>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Template, bool>> where = x => true;

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Template>(input.Page, input.PerPage, total, items!));
    }
}

