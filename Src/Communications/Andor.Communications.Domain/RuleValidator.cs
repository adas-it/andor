using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain;

public class RuleValidator
    : DefaultValidator<Rule, RuleId>, IRuleValidator
{
    public override async Task<List<Notification>> ValidateCreationAsync(Rule entity,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        AddNotification(entity.Name.Value.NotNullOrEmptyOrWhiteSpace(), notifications);
        AddNotification(entity.Name.Value.BetweenLength(2, 50), notifications);

        await DefaultValidationsAsync(entity, notifications, cancellationToken);

        return notifications;
    }
}
