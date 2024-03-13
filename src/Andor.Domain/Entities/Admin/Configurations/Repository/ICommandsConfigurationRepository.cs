using Andor.Domain.Entities.Admin.Configurations.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Entities.Admin.Configurations.Repository;

public interface ICommandsConfigurationRepository : ICommandRepository<Configuration, ConfigurationId>
{
}
