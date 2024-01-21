namespace Family.Budget.Application.Dto.PaymentMethods.Requests;

public record RegisterPaymentMethodInput : BasePaymentMethod
{
    public RegisterPaymentMethodInput(string name,
        string description,
        DateTimeOffset? startDate, 
        DateTimeOffset? deactivationDate) : base(name, description, startDate, deactivationDate)
    {
    }

    public RegisterPaymentMethodInput() : base()
    {
    }
}
