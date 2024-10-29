namespace Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;

public record PaymentMethodOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public int Order { get; set; }
}
