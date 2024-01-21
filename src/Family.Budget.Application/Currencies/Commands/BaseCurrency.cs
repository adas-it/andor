namespace Family.Budget.Application.Currencies.Commands;
using System;

public abstract record BaseCurrency
{
    protected BaseCurrency() { }
    protected BaseCurrency(string name, string iso)
    {
        Name = name;
        ISO = iso;
    }

    public string Name { get; set; }
    public string ISO { get; set; }
}
