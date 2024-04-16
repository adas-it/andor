namespace Andor.Domain.Engagement.Budget.Entities.Accounts.DomainEvents;

public sealed record AccountCreatedDomainEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";

    public static AccountCreatedDomainEvent FromAggregator(Account entity)
        => new AccountCreatedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name
        };
}
