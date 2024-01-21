namespace Family.Budget.Application.Dto.Categories.Requests;

public record RegisterCategoryInput : BaseCategory
{
    public RegisterCategoryInput(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        int movementTypeId) : base(name, description, startDate, deactivationDate, movementTypeId)
    {
    }

    public RegisterCategoryInput() : base()
    {
    }
}
