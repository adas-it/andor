namespace Family.Budget.Application.Dto.SubCategories.Requests;

public abstract record BaseSubCategory
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public Guid CategoryId { get; set; }

    public BaseSubCategory() { }

    public BaseSubCategory(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? finalDate,
        Guid categoryId)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = finalDate;
        CategoryId = categoryId;
    }
}
