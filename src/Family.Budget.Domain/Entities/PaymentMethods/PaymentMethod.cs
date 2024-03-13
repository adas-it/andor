namespace Family.Budget.Domain.Entities.PaymentMethods;

using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Entities.PaymentMethods.DomainEvents;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;
public class PaymentMethod : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? DeactivationDate { get; private set; }
    public MovementType Type { get; private set; }
    private PaymentMethod(Guid id, 
        string name, 
        string description,
        MovementType type,
        DateTimeOffset? startDate, 
        DateTimeOffset? deactivationDate)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        Type = type;

        Validate();
    }
    private PaymentMethod()
    { }

    public static PaymentMethod New(string name,
        string description, 
        MovementType movementType,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate)
    {
        var entity = new PaymentMethod(Guid.NewGuid(),
            name,
            description,
            movementType,
            startDate,
            deactivationDate);

        entity.RaiseDomainEvent(new PaymentMethodCreatedDomainEvent(entity));

        return entity;
    }

    protected override void Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(Description.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Description.BetweenLength(3, 1000));

        AddNotification(StartDate.NotDefaultDateTime());

        AddNotification(DeactivationDate.NotDefaultDateTime());

        if (DeactivationDate.GetValueOrDefault() != (default) && StartDate.GetValueOrDefault() != (default)
            && DeactivationDate!.Value < StartDate!)
        {
            //var message = DefaultsErrorsMessages.Date0CannotBeBeforeDate1.GetMessage(nameof(DeactivationDate), nameof(StartDate));

            //AddNotification(new (nameof(DeactivationDate), message, ErrorsCodes.CategoryDateConflit));
        }

        base.Validate();
    }
}
