using Andor.Domain.Communications;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Communication.Repositories;

public class QueriesPermissionRepository(PrincipalContext context) :
    QueryHelper<Permission, PermissionId>(context),
    IQueriesPermissionRepository
{
    public Task<SearchOutput<Permission>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Permission, bool>> where = x => true;

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Permission>(input.Page, input.PerPage, total, items!));
    }
}

