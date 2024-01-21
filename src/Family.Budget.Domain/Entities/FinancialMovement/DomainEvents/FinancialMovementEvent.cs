namespace Family.Budget.Domain.Entities.FinancialMovement.DomainEvents;

using Family.Budget.Domain.SeedWork;
public sealed record FinancialMovementChangedSubCategoryDomainEvent : DomainEventBase
{
	public FinancialMovementChangedSubCategoryDomainEvent(FinancialMovement entity) : base (nameof(FinancialMovementChangedSubCategoryDomainEvent))
	{
        Entity = entity;
    }

	public FinancialMovement Entity { get; set; }
}
public sealed record FinancialMovementChangedStatusDomainEvent : DomainEventBase
{
    public FinancialMovementChangedStatusDomainEvent(FinancialMovement entity) : base(nameof(FinancialMovementChangedStatusDomainEvent))
    {
        Entity = entity;
    }

    public FinancialMovement Entity { get; set; }
}
public sealed record FinancialMovementRemovedDomainEvent : DomainEventBase
{
    public FinancialMovementRemovedDomainEvent(FinancialMovement entity) : base(nameof(FinancialMovementRemovedDomainEvent))
    {
        Entity = entity;
    }

    public FinancialMovement Entity { get; set; }
}
public sealed record FinancialMovementCreatedDomainEvent : DomainEventBase
{
    public FinancialMovementCreatedDomainEvent(FinancialMovement entity) : base(nameof(FinancialMovementCreatedDomainEvent))
    {
        Entity = entity;
    }

    public FinancialMovement Entity { get; set; }
}
public sealed record FinancialMovementChangedDomainEvent : DomainEventBase
{
    public FinancialMovementChangedDomainEvent() : base(nameof(FinancialMovementChangedDomainEvent))
    {
    }

    public FinancialMovement Entity { get; set; }
    public FinancialMovement OldEntity { get; set; }
}