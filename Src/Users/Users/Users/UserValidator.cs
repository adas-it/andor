using Andor.Foundation.Domain.ValuesObjects;
using Users.Users.Errors;
using Users.Users.Repositories;
using Users.Users.ValueObjects;

namespace Users.Users;

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
