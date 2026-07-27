using Andor.Accounts.Contracts.FinancialMovementStatuses;
using Andor.Accounts.Contracts.MovementTypes;
using Andor.Accounts.Contracts.PaymentMethods.Responses;
using Andor.Accounts.Contracts.SubCategories.Responses;

namespace Andor.Accounts.Contracts.FinancialMovements.Response;

public record FinancialMovementOutput
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public SubCategoryOutput SubCategory { get; set; }
    public MovementTypeOutput Type { get; set; }
    public FinancialMovementStatusOutput Status { get; set; }
    public PaymentMethodOutput PaymentMethod { get; set; }
}
