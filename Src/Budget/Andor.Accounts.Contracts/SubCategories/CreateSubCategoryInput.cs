namespace Andor.Accounts.Contracts.SubCategories;

public record CreateSubCategoryInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? DefaultPaymentMethodId { get; set; }
}
