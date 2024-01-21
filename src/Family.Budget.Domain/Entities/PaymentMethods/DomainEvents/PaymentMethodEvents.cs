namespace Family.Budget.Domain.Entities.PaymentMethods.DomainEvents;

using Family.Budget.Domain.SeedWork;
public sealed record PaymentMethodChangedDomainEvent : DomainEventBase
{
    public PaymentMethodChangedDomainEvent(PaymentMethod entity) : base(nameof(PaymentMethodChangedDomainEvent))
    {
        Entity = entity;
    }

    public PaymentMethod Entity { get; set; }
}

public sealed record PaymentMethodCreatedDomainEvent : DomainEventBase
{
    public PaymentMethodCreatedDomainEvent(PaymentMethod entity) : base(nameof(PaymentMethodCreatedDomainEvent))
    {
        Entity = entity;
    }

    public PaymentMethod Entity { get; set; }
}