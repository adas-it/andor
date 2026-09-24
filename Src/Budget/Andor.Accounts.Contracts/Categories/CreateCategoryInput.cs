namespace Andor.Accounts.Contracts.Categories;

public record CreateCategoryInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int TypeId { get; set; }
}
