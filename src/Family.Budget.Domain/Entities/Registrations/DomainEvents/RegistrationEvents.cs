namespace Family.Budget.Domain.Entities.Registrations.DomainEvents;
using Family.Budget.Domain.SeedWork;

public sealed record RegistrationCreatedDomainEvent : DomainEventBase
{
    public RegistrationCreatedDomainEvent(Registration entity) : base(nameof(RegistrationCreatedDomainEvent))
    {
        Entity = entity;
    }

    public Registration Entity { get; set; }
}
public sealed record RegistrationCodeChangedDomainEvent : DomainEventBase
{
    public RegistrationCodeChangedDomainEvent(Registration entity) : base(nameof(RegistrationCodeChangedDomainEvent))
    {
        Entity = entity;
    }

    public Registration Entity { get; set; }
}