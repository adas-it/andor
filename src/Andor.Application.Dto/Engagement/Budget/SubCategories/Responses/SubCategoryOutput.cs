using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;

namespace Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;

public record SubCategoryOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public CategoryOutput Category { get; set; }
    public PaymentMethodOutput? DefaultPaymentMethod { get; set; }
    public int? Order { get; set; }
}
