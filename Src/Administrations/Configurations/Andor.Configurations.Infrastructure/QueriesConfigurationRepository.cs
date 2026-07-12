using System.Linq.Expressions;
using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Application.Queries;
using Andor.Configurations.Domain;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Configurations.Infrastructure.Context;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Andor.Configurations.Infrastructure;

public class QueriesConfigurationRepository(ConfigurationContext context) :
    QueryHelper<Configuration, ConfigurationId>(context), IQueriesConfigurationRepository
{
    public Task<List<Configuration>> GetByNameAndStatesAsync(Name name, ConfigurationState[] states,
        CancellationToken cancellationToken)
    {
        List<Expression<Func<Configuration, bool>>> where = BuildConfigurationFilters(name, states);

        return GetMany(where).ToListAsync(cancellationToken);
    }

    public async Task<SearchOutput<Configuration>> SearchAsync(SearchConfigurationInput input,
    CancellationToken cancellationToken)
    {
        string name = input.Search ?? "";
        ConfigurationState[] states = input.States ?? Array.Empty<ConfigurationState>();

        List<Expression<Func<Configuration, bool>>> where = BuildConfigurationFilters(name, states);

        var items = await GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToListAsync(cancellationToken);

        return new SearchOutput<Configuration>(input.Page, input.PerPage, total, items!);
    }

    private static List<Expression<Func<Configuration, bool>>> BuildConfigurationFilters(string name, ConfigurationState[] states)
    {
        var now = DateTime.UtcNow;
        List<Expression<Func<Configuration, bool>>> where = new();

        if (!string.IsNullOrWhiteSpace(name))
            where.Add(x => x.Name == name.ToLower().Trim());

        if (states.Any() == true)
        {
            Expression<Func<Configuration, bool>> stateFilter = null;

            if (states.Contains(ConfigurationState.Awaiting))
            {
                var awaiting = (Expression<Func<Configuration, bool>>)(x => x.StartDate > now);
                stateFilter = stateFilter == null ? awaiting : stateFilter.Or(awaiting);
            }

            if (states.Contains(ConfigurationState.Active))
            {
                var active = (Expression<Func<Configuration, bool>>)(x =>
                    x.StartDate <= now && (x.ExpireDate == null || x.ExpireDate > now));
                stateFilter = stateFilter == null ? active : stateFilter.Or(active);
            }

            if (states.Contains(ConfigurationState.Expired))
            {
                var expired = (Expression<Func<Configuration, bool>>)(x =>
                    x.ExpireDate != null && x.ExpireDate <= now);
                stateFilter = stateFilter == null ? expired : stateFilter.Or(expired);
            }

            if (stateFilter != null)
                where.Add(stateFilter);
        }

        return where;
    }
}
