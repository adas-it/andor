using Andor.Configurations.Application.Commands;
using Andor.Configurations.Contracts.Responses;
using Andor.Foundation.Contracts.Results;

namespace Andor.Configurations.Application.Interfaces;

public interface IConfigurationCommandsService
{
    Task<ApplicationResult<ConfigurationOutput?>> ChangeConfigurationAsync(ChangeConfigurationCommand command);

    Task<ApplicationResult<ConfigurationOutput?>> CreateConfigurationAsync(CreateConfigurationCommand command);

    Task<ApplicationResult<object?>> DeleteConfigurationAsync(DeleteConfigurationCommand command);

    Task<ApplicationResult<object?>> DeactivateConfigurationAsync(DeactivateConfigurationCommand command);
}
