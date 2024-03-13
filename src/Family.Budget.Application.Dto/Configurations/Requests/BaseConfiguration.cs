namespace Family.Budget.Application.Dto.Configurations.Requests;

public abstract class BaseConfiguration
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset FinalDate { get; set; }

    public BaseConfiguration()
    {
        Name = string.Empty;
        Value = string.Empty;
        Description = string.Empty;
    }

    public BaseConfiguration(string name,
        string value,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset finalDate)
    {
        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        FinalDate = finalDate;
    }
}
