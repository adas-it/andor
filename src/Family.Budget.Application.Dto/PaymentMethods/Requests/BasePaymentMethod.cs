namespace Family.Budget.Application.Dto.PaymentMethods.Requests;

public abstract record BasePaymentMethod
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }

    public BasePaymentMethod(string name, string description, DateTimeOffset? startDate, DateTimeOffset? deactivationDate)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
    }

    public BasePaymentMethod() { }

}
