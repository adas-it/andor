using Andor.Domain.Entities.Admin.Configurations;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Entities.Admin.Configurations.ValueObjects;
using Andor.Domain.Onboarding.Registrations.Repositories.Models;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Administrations.Configurations;

public class QueriesConfigurationRepository(PrincipalContext context) :
    QueryHelper<Configuration, ConfigurationId>(context),
    IQueriesConfigurationRepository
{
    public Task<SearchOutput<Configuration>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Configuration, bool>> where = x => x.IsDeleted == false;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Configuration>(input.Page, input.PerPage, total, items!));
    }

    public async Task<List<Configuration>?> GetByNameAndStatusAsync(SearchConfigurationInput search,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();
        query = GetWhere(query, search);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Configuration?> GetActiveByNameAsync(string name, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();
        query = GetWhere(query, new SearchConfigurationInput(name, [ConfigurationState.Active]));

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<Configuration> GetWhere(IQueryable<Configuration> sourceQuery, SearchConfigurationInput search)
    {
        if (!string.IsNullOrEmpty(search.Name))
        {
            sourceQuery = sourceQuery.Where(x => x.Name.Equals(search.Name));
        }

        if (search.States.Contains(ConfigurationState.Expired))
        {
            sourceQuery = sourceQuery.Where(x => x.ExpireDate > DateTime.UtcNow);
        }

        if (!search.States.Contains(ConfigurationState.Awaiting))
        {
            sourceQuery = sourceQuery.Where(x => x.StartDate < DateTime.UtcNow);
        }

        if (!search.States.Contains(ConfigurationState.Active))
        {
            sourceQuery = sourceQuery.Where(x => x.StartDate < DateTime.UtcNow && (x.ExpireDate == null || x.ExpireDate > DateTime.UtcNow));
        }

        return sourceQuery;
    }
}

