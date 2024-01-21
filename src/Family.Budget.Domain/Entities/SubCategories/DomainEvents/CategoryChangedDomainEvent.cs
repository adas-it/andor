namespace Family.Budget.Domain.Entities.SubCategories.DomainEvents;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.SeedWork;
public sealed record SubCategoryChangedDomainEvent: DomainEventBase
{
    public SubCategoryChangedDomainEvent(SubCategory entity) : base(nameof(SubCategoryChangedDomainEvent))
    {
        Entity = entity;
    }

    public SubCategory Entity { get; set; }
}
