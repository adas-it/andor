namespace Andor.Accounts.Contracts.PaymentMethods;

public record CreatePaymentMethodInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int TypeId { get; set; }
}
