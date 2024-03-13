namespace Family.Budget.Domain.Entities.SubCategories.DomainEvents;
using Family.Budget.Domain.SeedWork;
public sealed record SubCategoryCreatedDomainEvent : DomainEventBase
{

    public SubCategoryCreatedDomainEvent(SubCategory entity) : base(nameof(SubCategoryCreatedDomainEvent))
    {
        Entity = entity;
    }

    public SubCategory Entity { get; set; }
}
