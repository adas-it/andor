using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Domain;

public class OnboardingValidator
    : DefaultValidator<SignupRequest, SignupRequestId>, IOnboardingValidator
{
    public override async Task<List<Notification>> ValidateCreationAsync(SignupRequest entity,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        AddNotification(entity.Name.Value.NotNullOrEmptyOrWhiteSpace(), notifications);
        AddNotification(entity.Email.NotNullOrEmptyOrWhiteSpace(), notifications);

        await DefaultValidationsAsync(entity, notifications, cancellationToken);

        return notifications;
    }
}
