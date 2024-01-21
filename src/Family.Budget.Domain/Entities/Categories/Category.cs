namespace Family.Budget.Domain.Entities.Categories;

using Family.Budget.Domain.Common;
using Family.Budget.Domain.Entities.Categories.DomainEvents;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? DeactivationDate { get; private set; }
    public MovementType Type { get; private set; }

    private Category()
    {
    }

    private Category(Guid id, 
        string name, 
        string description, 
        DateTimeOffset? startDate, 
        DateTimeOffset? deactivationDate,
        MovementType type)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        Type = type;

        Validate();
    }

    public static Category New(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        MovementType type)
    {
        var entity = new Category(Guid.NewGuid(),
            name,
            description,
            startDate,
            deactivationDate,
            type);

        entity.RaiseDomainEvent(new CategoryCreatedDomainEvent(entity));

        return entity;
    }

    protected override void Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(StartDate.NotDefaultDateTime());

        AddNotification(DeactivationDate.NotDefaultDateTime());

        if (DeactivationDate.GetValueOrDefault() != (default) && StartDate.GetValueOrDefault() != (default)
            && DeactivationDate!.Value < StartDate!)
        {
            var message = DefaultsErrorsMessages.Date0CannotBeBeforeDate1.GetMessage(nameof(DeactivationDate), nameof(StartDate));
         
            AddNotification(new (nameof(DeactivationDate), message, CommonErrorCodes.Validation));
        }

        AddNotification(Type.NotNull());

        base.Validate();
    }

    public void SetCategory(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;

        Validate();

        RaiseDomainEvent(new CategoryChangedDomainEvent(this));
    }
}
