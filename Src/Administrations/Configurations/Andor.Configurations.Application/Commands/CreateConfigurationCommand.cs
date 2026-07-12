using Andor.Authorizations.Domain;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Application.Commands;

public record CreateConfigurationCommand(ConfigurationId Id, Name Name, Value Value, Description Description,
    DateTime StartDate, DateTime? ExpireDate, bool Force, ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<ConfigurationId>
{
}
