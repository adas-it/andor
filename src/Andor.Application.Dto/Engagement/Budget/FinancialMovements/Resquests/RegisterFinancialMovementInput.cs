namespace Andor.Application.Dto.Engagement.Budget.FinancialMovements.Resquests;

public record RegisterFinancialMovementInput
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public Guid SubCategoryId { get; set; }
    public int StatusId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid AccountId { get; set; }
}
