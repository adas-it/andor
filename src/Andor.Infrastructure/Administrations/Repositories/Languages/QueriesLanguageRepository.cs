using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.Repositories;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Administrations.Repositories.Configurations;

public class QueriesLanguageRepository(PrincipalContext context) :
    QueryHelper<Language, LanguageId>(context),
    IQueriesLanguageRepository
{
    public Task<SearchOutput<Language>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Language, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Language>(input.Page, input.PerPage, total, items!));
    }
}

