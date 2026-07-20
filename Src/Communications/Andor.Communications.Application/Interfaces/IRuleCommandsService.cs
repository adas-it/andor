using Andor.Communications.Application.Commands;
using Andor.Communications.Contracts.Responses;
using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Application.Interfaces;

public interface IRuleCommandsService
{
    Task<ApplicationResult<RuleOutput?>> CreateRuleAsync(CreateRuleCommand command);

    Task<ApplicationResult<object?>> SendNotificationAsync(SendNotificationCommand command);
}
