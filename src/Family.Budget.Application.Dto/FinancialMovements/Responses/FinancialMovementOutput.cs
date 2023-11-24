using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Dto.MovementStatuses;
using Family.Budget.Application.Dto.MovementTypes;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Dto.SubCategories.Responses;

namespace Family.Budget.Application.Dto.FinancialMovements.Responses;

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
    public FinancialMovementOutput() { }

    public FinancialMovementOutput(Guid id,
        DateTime? date,
        string? description,
        decimal value,
        SubCategoryOutput subCategory,
        MovementTypeOutput type,
        FinancialMovementStatusOutput status,
        PaymentMethodOutput paymentMethod)
    {
        Id = id;
        Date = date;
        Description = description;
        Value = value;
        SubCategory = subCategory;
        Type = type;
        Status = status;
        PaymentMethod = paymentMethod;
    }
}
