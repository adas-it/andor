using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;

public class PaymentMethod : AggregateRoot<PaymentMethodId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? DeactivationDate { get; private set; }
    public MovementType Type { get; private set; } = MovementType.Undefined;
    public int? DefaultOrder { get; private set; }

    public ICollection<SubCategory> SubCategories { get; set; }

    private PaymentMethod()
    {
        Id = PaymentMethodId.New();
        Name = string.Empty;
        Description = string.Empty;
        SubCategories = [];
    }

    private DomainResult SetValues(PaymentMethodId id,
        string name,
        int order)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 70));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Name = name;
        DefaultOrder = order;

        var result = Validate();

        return result;
    }

    public static (DomainResult, PaymentMethod?) New(
        string name,
        int order)
    {
        var entity = new PaymentMethod();

        var result = entity.SetValues(PaymentMethodId.New(), name, order);

        if (result.IsFailure)
        {
            return (result, null);
        }

        return (result, entity);
    }
}
