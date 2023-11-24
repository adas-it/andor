namespace Family.Budget.Application.PaymentMethod.Commands;
using System;

public abstract record BasePaymentMethod
{
    protected BasePaymentMethod() { }
    protected BasePaymentMethod(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
}
