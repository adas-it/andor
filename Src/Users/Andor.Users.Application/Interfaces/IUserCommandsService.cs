using Andor.Users.Application.Commands;
using Andor.Users.Contracts.Responses;
using Andor.Foundation.Contracts.Results;

namespace Andor.Users.Application.Interfaces;

public interface IUserCommandsService
{
    Task<ApplicationResult<UserPreferencesOutput?>> CreateUserAsync(CreateUserCommand command);
}
