using Andor.Configurations.Application.Queries;
using Andor.Configurations.Contracts.Responses;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Configurations.Application.Interfaces;

public interface IConfigurationQueriesService
{
    Task<ApplicationResult<ConfigurationOutput?>> GetByIdAsync(ConfigurationId id, CancellationToken cancellationToken);

    Task<ApplicationResult<SearchOutput<ConfigurationOutput>>> SearchAsync(SearchConfigurationInput input, CancellationToken cancellationToken);
}
