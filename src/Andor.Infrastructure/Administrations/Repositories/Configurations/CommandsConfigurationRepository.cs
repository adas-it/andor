using Andor.Domain.Entities.Admin.Configurations;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Entities.Admin.Configurations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Administrations.Repositories.Configurations;

public class CommandsConfigurationRepository(PrincipalContext context) :
    CommandsBaseRepository<Configuration, ConfigurationId>(context),
    ICommandsConfigurationRepository
{
}
