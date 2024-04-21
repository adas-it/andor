using Andor.Application.Dto.Engagement.Budget.FinancialMovementStatuses;
using Andor.Application.Dto.Engagement.Budget.MovementTypes;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;

namespace Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;

public record FinancialMovementOutput
{
    public Guid Id { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public SubCategoryOutput SubCategory { get; set; }
    public MovementTypeOutput Type { get; set; }
    public FinancialMovementStatusOutput Status { get; set; }
    public PaymentMethodOutput PaymentMethod { get; set; }
}
