namespace Andor.Application.Dto.Engagement.Budget.PaymentMethods.Requests;

public record ModifyPaymentMethodInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
}
