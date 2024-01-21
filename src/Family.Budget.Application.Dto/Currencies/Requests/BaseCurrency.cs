namespace Family.Budget.Application.Dto.Currencies.Requests;

public abstract record BaseCurrency
{
    public string Name { get; set; }
    public string ISO { get; set; }

    public BaseCurrency() { }

    public BaseCurrency(string name,
        string iso)
    {
        Name = name;
        ISO = iso;
    }
}
