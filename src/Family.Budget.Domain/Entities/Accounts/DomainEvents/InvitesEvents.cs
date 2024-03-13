namespace Family.Budget.Domain.Entities.Accounts.DomainEvents;

using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.SeedWork;

public record InviteCreatedDomainEvent : DomainEventBase
{
    public InviteCreatedDomainEvent(Invite entity) : base(nameof(InviteCreatedDomainEvent))
    {
        Entity = entity;
    }

    public Invite Entity { get; set; }
}
