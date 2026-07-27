using Andor.Accounts.Contracts.Categories.Response;
using Andor.Accounts.Contracts.PaymentMethods.Responses;

namespace Andor.Accounts.Contracts.SubCategories.Responses;

public record SubCategoryOutput
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CategoryOutput Category { get; set; }
    public PaymentMethodOutput? DefaultPaymentMethod { get; set; }
    public int? Order { get; set; }
}
