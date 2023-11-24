namespace Family.Budget.Application.Dto.FinancialMovements.Requests;

public abstract record BaseFinancialMovement
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public Guid SubCategoryId { get; set; }
    public int StatusId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid AccountId { get; set; }
}
