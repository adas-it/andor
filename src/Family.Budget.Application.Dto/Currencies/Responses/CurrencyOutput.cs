namespace Family.Budget.Application.Dto.Currencies.Responses;

public record CurrencyOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ISO { get; set; }

    public CurrencyOutput() { }

    public CurrencyOutput(Guid id, string name, string iso)
    {
        Id = id;
        ISO = iso;
        Name = name;
    }
}
