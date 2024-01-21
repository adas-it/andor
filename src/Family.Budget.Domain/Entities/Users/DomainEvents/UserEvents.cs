namespace Family.Budget.Domain.Entities.Users.DomainEvents;

using Family.Budget.Domain.SeedWork;
public sealed record UserCreatedDomainEvent : DomainEventBase
{

    public UserCreatedDomainEvent(User entity) : base(nameof(UserCreatedDomainEvent))
    {
        Entity = entity;
    }

    public User Entity { get; set; }
}
