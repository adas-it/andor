using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Entities.Currencies.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Currencies;

public class Currency : Entity<CurrencyId>
{
    public string Name { get; private set; } = string.Empty;
    public Iso Iso { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;

    private Currency()
    {
        Validate();
    }

    public static Currency New(string name,
        string iso,
        string symbol)
    {
        var entity = new Currency()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Iso = iso,
            Symbol = symbol
        };

        return entity;
    }

    protected override DomainResult Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(Iso.NotNull());

        return base.Validate();
    }
}

