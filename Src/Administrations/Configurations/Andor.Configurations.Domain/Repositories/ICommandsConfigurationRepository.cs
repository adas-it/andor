using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Domain.Repositories;

public interface ICommandsConfigurationRepository :
    ICommandRepository<Configuration, ConfigurationId>
{

    Task<List<Configuration>> GetByNameAndStatesAsync(Name name, ConfigurationState[] states,
        CancellationToken cancellationToken);
}
