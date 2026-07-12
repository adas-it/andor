using Andor.Configurations.Application.Queries;
using Andor.Configurations.Domain;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Application.Interfaces;

public interface IQueriesConfigurationRepository :
    ISearchableRepository<Configuration, ConfigurationId, SearchConfigurationInput>
{
    Task<List<Configuration>> GetByNameAndStatesAsync(Name name, ConfigurationState[] states,
        CancellationToken cancellationToken);
}
