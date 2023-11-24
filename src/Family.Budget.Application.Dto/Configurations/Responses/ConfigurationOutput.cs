namespace Family.Budget.Application.Dto.Configurations.Responses;

public record ConfigurationOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? FinalDate { get; set; }

    public ConfigurationOutput()
    {
        Name = string.Empty;
        Value = string.Empty;
        Description = string.Empty;
    }

    public ConfigurationOutput(Guid id, 
        string name, 
        string value, 
        string description, 
        DateTimeOffset startDate, 
        DateTimeOffset? finalDate)
    {
        Id = id;
        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        FinalDate = finalDate;
    }
}
