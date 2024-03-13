using Family.Budget.Domain.Entities.Currencies;

namespace Family.Budget.Domain.Entities.Users.ValueObject;
public sealed record LocationInfos
{
    public LocationInfos(string preferedLanguage, Currency localCurrency)
    {
        PreferedLanguage = preferedLanguage;
        LocalCurrency = localCurrency;
    }

    public string PreferedLanguage { get; private set; }
    public Currency LocalCurrency { get; private set; }

    public static LocationInfos New(string preferedLanguage, Currency localCurrency)
        => new LocationInfos(preferedLanguage, localCurrency);

    private void Validate()
    {
    }
}
