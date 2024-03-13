namespace Family.Budget.Domain.Entities.Currencies;

using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;

public class Currency : Entity
{
    public string Name { get; private set; }
    public string Iso { get; private set; }
    public string Symbol { get; private set; }

    private Currency()
    {
    }

    private Currency(Guid id, string name, string iso, string symbol)
    {
        Id = id;
        Name = name;
        Iso = iso;
        Symbol = symbol;

        Validate();
    }

    public static Currency New(string name,
        string iso,
        string symbol)
    {
        var entity = new Currency(Guid.NewGuid(),
            name,
            iso,
            symbol);

        return entity;
    }

    protected override void Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(Iso.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Iso.BetweenLength(3, 3));

        base.Validate();
    }

    public void SetCurrency(string name,
        string iso)
    {
        Name = name;
        Iso = iso;

        Validate();
    }
}
