namespace Family.Budget.Domain.Entities.Accounts.DomainEvents;

using Family.Budget.Domain.SeedWork;

public sealed record AccountCreatedDomainEvent : DomainEventBase
{
    public AccountCreatedDomainEvent(Account entity) : base(nameof(AccountCreatedDomainEvent))
    {
        Entity = entity;
    }

    public Account Entity { get; set; }
}

public sealed record AccountDeletedDomainEvent : DomainEventBase
{
    public AccountDeletedDomainEvent(Account entity) : base(nameof(AccountCreatedDomainEvent))
    {
        Entity = entity;
    }

    public Account Entity { get; set; }
}
