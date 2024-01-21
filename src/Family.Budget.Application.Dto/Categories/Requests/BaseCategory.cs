namespace Family.Budget.Application.Dto.Categories.Requests;

public abstract record BaseCategory
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; private set; }
    public int MovementTypeId { get; private set; }

    public BaseCategory() { }

    public BaseCategory(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? finalDate,
        int typeId)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = finalDate;
        MovementTypeId = typeId;
    }
}
