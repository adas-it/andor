namespace Family.Budget.Application.SubCategories.Commands;
using System;

public abstract record BaseSubCategory
{
    protected BaseSubCategory() { }
    protected BaseSubCategory(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        Guid categoryId)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        CategoryId = categoryId;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public Guid CategoryId { get; set; }
}
