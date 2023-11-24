using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Dto.PaymentMethods.Responses;

namespace Family.Budget.Application.Dto.SubCategories.Responses;

public record SubCategoryOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public CategoryOutput Category { get; set; }
    public PaymentMethodOutput? DefaultPaymentMethod { get; set; }

public SubCategoryOutput() { }

    public SubCategoryOutput(Guid id, 
        string name, 
        string description, 
        DateTimeOffset? startDate, 
        DateTimeOffset? deactivationDate,
        CategoryOutput category, PaymentMethodOutput? defaultPaymentMethod)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        Category= category;
        DefaultPaymentMethod = defaultPaymentMethod;
    }
}
