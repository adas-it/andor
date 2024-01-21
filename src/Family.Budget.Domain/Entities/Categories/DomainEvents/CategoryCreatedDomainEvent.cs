namespace Family.Budget.Domain.Entities.Categories.DomainEvents;

using Family.Budget.Domain.SeedWork;
public sealed record CategoryCreatedDomainEvent : DomainEventBase
{
	public CategoryCreatedDomainEvent(Category category) : base(nameof(CategoryCreatedDomainEvent))
	{
        Entity = category;
    }

    public Category Entity { get; set; }
}
