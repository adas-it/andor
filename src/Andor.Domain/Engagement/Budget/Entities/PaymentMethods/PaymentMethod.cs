using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.MovementTypes;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.PaymentMethods;

public class PaymentMethod : AggregateRoot<PaymentMethodId>
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";
    public DateTime? StartDate { get; private set; }
    public DateTime? DeactivationDate { get; private set; }
    public MovementType Type { get; private set; } = MovementType.Undefined;
    private DomainResult SetValues(PaymentMethodId id,
        string name)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 70));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Name = name;

        var result = Validate();

        return result;
    }

    public static (DomainResult, PaymentMethod?) New(
        string name)
    {
        var entity = new PaymentMethod();

        var result = entity.SetValues(PaymentMethodId.New(), name);

        if (result.IsFailure)
        {
            return (result, null);
        }

        return (result, entity);
    }
}
