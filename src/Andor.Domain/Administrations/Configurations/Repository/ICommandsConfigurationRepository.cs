using Andor.Domain.Administrations.Configurations.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Administrations.Configurations.Repository;

public interface ICommandsConfigurationRepository : ICommandRepository<Configuration, ConfigurationId>
{
}
