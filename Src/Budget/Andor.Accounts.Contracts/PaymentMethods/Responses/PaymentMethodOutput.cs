namespace Andor.Accounts.Contracts.PaymentMethods.Responses;

public record PaymentMethodOutput
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Order { get; set; }
    public bool IsTemplate { get; set; }
}
