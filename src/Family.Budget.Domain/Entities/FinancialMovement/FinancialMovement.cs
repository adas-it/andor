namespace Family.Budget.Domain.Entities.FinancialMovement;

using Family.Budget.Domain.Entities.FinancialMovement.DomainEvents;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;

public class FinancialMovement : AggregateRoot
{
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }
    public SubCategory SubCategory { get; private set; }
    public MovementType Type { get; private set; }
    public MovementStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Value { get; private set; }
    public bool IsDeleted { get; private set; }

    private FinancialMovement()
    {
    }

    private FinancialMovement(Guid id, DateTime date, string? description,
        SubCategory subCategory, MovementType type, MovementStatus status,
        PaymentMethod paymentMethod, Guid accountId, decimal value)
    {
        Id = id;
        Description = description;
        SubCategory = subCategory;
        Type = type;
        Status = status;
        PaymentMethod = paymentMethod;
        Date = date;
        AccountId = accountId;
        Value = value;

        Validate();
    }

    public static FinancialMovement New(DateTime date, string? description,
        decimal value, SubCategory subCategory, MovementType type,
        MovementStatus status, PaymentMethod paymentMethod, Guid accountId)
    {
        var entity = new FinancialMovement(Guid.NewGuid(),
            date,
            description,
            subCategory,
            type,
            status,
            paymentMethod,
            accountId,
            value);

        entity.RaiseDomainEvent(new FinancialMovementCreatedDomainEvent(entity));

        return entity;
    }
    protected override void Validate()
    {
        AddNotification(Description.BetweenLength(3, 100));

        base.Validate();
    }

    public void SetNewValues(DateTime date, string? description,
        decimal value, SubCategory subCategory, MovementType type,
        MovementStatus status, PaymentMethod paymentMethod)
    {
        var oldEntity = New(Date, Description, Value, SubCategory, Type, Status, PaymentMethod, AccountId);

        Description = description;
        Value = value;
        SubCategory = subCategory;
        Type = type;
        Status = status;
        PaymentMethod = paymentMethod;
        Date = date;

        RaiseDomainEvent(new FinancialMovementChangedDomainEvent()
        {
            Entity = this,
            OldEntity = oldEntity,
        });

        base.Validate();
    }

    public void SetDeleted()
    {
        IsDeleted = true;
        RaiseDomainEvent(new FinancialMovementRemovedDomainEvent(this));
    }
}
