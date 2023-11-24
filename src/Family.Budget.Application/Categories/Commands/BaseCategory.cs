namespace Family.Budget.Application.Categories.Commands;
using System;

public abstract record BaseCategory
{
    protected BaseCategory() { }
    protected BaseCategory(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        int typeId)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        MovementTypeId = typeId;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public int MovementTypeId { get; set; }
}
