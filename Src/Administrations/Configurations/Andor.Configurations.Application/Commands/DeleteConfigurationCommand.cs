using Andor.Authorizations.Domain;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;

namespace Andor.Configurations.Application.Commands;

public record DeleteConfigurationCommand(ConfigurationId Id, ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<ConfigurationId>;
