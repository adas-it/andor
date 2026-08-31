using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Users.Application.Commands;
using Andor.Users.Application.Interfaces;
using Andor.Users.Contracts.Responses;
using Andor.Users.Domain.Users;
using Andor.Users.Domain.Users.Repositories;

namespace Andor.Users.Application;

public class UserCommandsService(
    ICommandsUserRepository repository,
    IUserValidator validator) : IUserCommandsService
{
    public async Task<ApplicationResult<UserPreferencesOutput?>> CreateUserAsync(CreateUserCommand command)
    {
        var response = ApplicationResult<UserPreferencesOutput?>.Success();

        var existing = await repository.GetByIdAsync(command.Id, command.CancellationToken);

        if (existing is not null)
        {
            return response.SetData(existing.ToUserPreferencesOutput());
        }

        var (result, user) = await User.NewAsync(
            command.Id,
            command.Email,
            command.FirstName,
            command.LastName,
            command.PreferredCurrencyId,
            command.PreferredLanguageId,
            validator,
            command.CancellationToken);

        if (result.IsFailure || user is null)
        {
            foreach (var _ in result.Errors)
            {
                response.AddError(Errors.Validation());
            }

            return response;
        }

        await repository.PersistAsync(user, command.CancellationToken);

        return response.SetData(user.ToUserPreferencesOutput());
    }
}
