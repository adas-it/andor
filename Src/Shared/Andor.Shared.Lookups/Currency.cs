namespace Andor.Shared.Lookups;

public class Currency
{
    public Guid Id { get; init; }
    public string ISO { get; init; }
    public string Name { get; init; }
    public string Symbol { get; init; }

    private Currency(Guid id, string iso, string name, string symbol)
    {
        Id = id;
        ISO = iso;
        Name = name;
        Symbol = symbol;
    }

    public static List<Currency> GetAll()
    {
        return new List<Currency>
        {
            new Currency(Guid.Parse("01994953-f745-73f4-85b3-03b5aad12591"), "USD", "US Dollar", "$"),
            new Currency(Guid.Parse("23c4a34f-53c5-44e3-aca6-a6733358e906"), "BRL", "Real Brasileiro", "R$"),
            new Currency(Guid.Parse("81f2562e-c81f-4ab8-8a78-184cba8269f9"), "EUR", "Euro", "€")
        };
    }

    public static Currency GetByISO(string? iso)
    {
        var currency = GetAll();
        return currency.FirstOrDefault(lang => lang.ISO.Equals(iso, StringComparison.OrdinalIgnoreCase)) ?? currency.First();
    }

}
