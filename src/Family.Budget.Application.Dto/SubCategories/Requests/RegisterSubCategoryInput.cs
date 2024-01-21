namespace Family.Budget.Application.Dto.SubCategories.Requests;

public record RegisterSubCategoryInput : BaseSubCategory
{
    public RegisterSubCategoryInput(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        Guid categoryId) : base(name, description, startDate, deactivationDate, categoryId)
    {
    }

    public Guid DefaultPaymentMethodId { get; set; }

    public RegisterSubCategoryInput() : base()
    {
    }
}
