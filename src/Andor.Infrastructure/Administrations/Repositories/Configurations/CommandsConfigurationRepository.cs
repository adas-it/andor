using Andor.Domain.Administrations.Configurations;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Administrations.Configurations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Administrations.Repositories.Configurations;

public class CommandsConfigurationRepository(PrincipalContext context) :
    CommandsBaseRepository<Configuration, ConfigurationId>(context),
    ICommandsConfigurationRepository
{
}
