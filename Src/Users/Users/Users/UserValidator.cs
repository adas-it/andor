using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Domain.Users.Errors;
using Andor.Users.Domain.Users.Repositories;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Domain.Users;

public class UserValidator(ICommandsUserRepository userRepository)
    : DefaultValidator<User, UserId>, IUserValidator
{
    public override async Task<List<Notification>> ValidateCreationAsync(
        User entity,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        var existingUser = await userRepository.GetByMailAsync(entity.Email, cancellationToken);

        if (existingUser is not null)
        {
            notifications.Add(new Notification(
                nameof(entity.Email),
                UserErrorMessages.EmailAlreadyInUse,
                UserErrorCode.EmailAlreadyInUse));
        }

        return notifications;
    }
}
