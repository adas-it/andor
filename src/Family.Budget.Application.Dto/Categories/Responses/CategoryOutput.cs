namespace Family.Budget.Application.Dto.Categories.Responses;

public record CategoryOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public string Avatar { get; set; }

    public CategoryOutput() { }

    public CategoryOutput(Guid id, 
        string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        string avatar)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        Avatar = avatar;
    }
}
