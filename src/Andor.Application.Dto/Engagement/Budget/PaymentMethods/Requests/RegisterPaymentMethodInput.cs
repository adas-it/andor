namespace Andor.Application.Dto.Engagement.Budget.PaymentMethods.Requests;

public record RegisterPaymentMethodInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
}
