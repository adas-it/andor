namespace Family.Budget.Application.Dto.PaymentMethods.Responses;

public record PaymentMethodOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }

    public PaymentMethodOutput() { }

    public PaymentMethodOutput(Guid id,
        string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
    }
}
