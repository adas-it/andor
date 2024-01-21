namespace Family.Budget.Application.Administrations.Commands;
using System;

public abstract record BaseConfiguration
{
    protected BaseConfiguration() { }
    protected BaseConfiguration(string name, string value, string description, DateTimeOffset startDate, DateTimeOffset? finalDate)
    {
        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        FinalDate = finalDate;
    }

    public string Name { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? FinalDate { get; set; }
}
