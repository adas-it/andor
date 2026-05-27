using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Currencies;

public class Currency : Entity<CurrencyId>
{
    public static readonly Currency Empty = new();
    public Name Name { get; private set; }
    public string Iso { get; private set; }
    public string Symbol { get; private set; }

    private Currency()
    {
        Name = null!;
        Iso = string.Empty;
        Symbol = string.Empty;
    }

    private DomainResult SetValues(
        Name name,
        string iso,
        string symbol)
    {
        AddNotification(name.NotNull());

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Name = name;
        Iso = iso;
        Symbol = symbol;

        var result = Validate();

        return result;
    }

    public static (DomainResult, Currency?) New(
        Name name,
        string iso,
        string symbol)
    {
        var entity = new Currency();

        var result = entity.SetValues(name, iso, symbol);

        if (result.IsFailure)
        {
            return (result, null);
        }

        return (result, entity);
    }
}
