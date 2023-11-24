namespace Family.Budget.Domain.Entities.Categories.DomainEvents;

using Family.Budget.Domain.SeedWork;
public sealed record CategoryChangedDomainEvent : DomainEventBase
{
	public CategoryChangedDomainEvent(Category entity) : base (nameof(CategoryChangedDomainEvent))
	{
        Entity = entity;
    }

    public Category Entity { get; set; }
}
