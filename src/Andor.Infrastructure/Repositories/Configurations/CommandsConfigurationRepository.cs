﻿using Andor.Domain.Entities.Admin.Configurations;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Repositories.Configurations;

public class CommandsConfigurationRepository(PrincipalContext context) :
    CommandsBaseRepository<Configuration, ConfigurationId>(context),
    ICommandsConfigurationRepository
{
}

