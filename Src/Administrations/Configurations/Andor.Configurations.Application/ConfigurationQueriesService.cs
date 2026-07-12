using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Application.Queries;
using Andor.Configurations.Contracts.Responses;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Configurations.Application;

public class ConfigurationQueriesService(
    IQueriesConfigurationRepository queriesConfigurationRepository)
    : IConfigurationQueriesService
{
    public async Task<ApplicationResult<ConfigurationOutput?>> GetByIdAsync(ConfigurationId id,
        CancellationToken cancellationToken)
    {
        var result = await queriesConfigurationRepository.GetByIdAsync(id, cancellationToken);
        return result.ToConfigurationOutput();
    }

    public async Task<ApplicationResult<SearchOutput<ConfigurationOutput>>> SearchAsync(SearchConfigurationInput input,
        CancellationToken cancellationToken)
    {
        var result = await queriesConfigurationRepository.SearchAsync(input, cancellationToken);

        return new SearchOutput<ConfigurationOutput>(result.CurrentPage, result.PerPage, result.Total,
            result.Items.Select(x => x.ToConfigurationOutput()).ToList());
    }
}
