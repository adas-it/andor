using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.Currencies;

public class Currency : Entity<CurrencyId>
{
    public string Name { get; private set; } = "";
    public string Iso { get; private set; } = "";
    public string Symbol { get; private set; } = "";
    private DomainResult SetValues(CurrencyId id,
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

    public static (DomainResult, Currency?) New(
        string name)
    {
        var entity = new Currency();

        var result = entity.SetValues(CurrencyId.New(), name);

        if (result.IsFailure)
        {
            return (result, null);
        }

        return (result, entity);
    }
}
