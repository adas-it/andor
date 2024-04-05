﻿using Andor.Domain.Entities.Communications;
using Andor.Domain.Entities.Communications.Repositories;
using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
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

