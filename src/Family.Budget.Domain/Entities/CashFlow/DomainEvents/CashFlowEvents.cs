namespace Family.Budget.Domain.Entities.CashFlow.DomainEvents;

using Family.Budget.Domain.SeedWork;

public record CashFlowEventsCreatedDomainEvent : DomainEventBase
{
	public CashFlowEventsCreatedDomainEvent(CashFlow entity) : base(nameof(CashFlowEventsCreatedDomainEvent))
    {
        Entity = entity;
    }

	public CashFlow Entity { get; set; }
}
public record AccountBalanceChangedDomainEvent : DomainEventBase
{
    public AccountBalanceChangedDomainEvent(CashFlow entity) : base(nameof(AccountBalanceChangedDomainEvent))
    {
        Entity = entity;
    }

    public CashFlow Entity { get; set; }
}
