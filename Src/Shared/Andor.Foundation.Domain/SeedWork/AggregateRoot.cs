using Andor.Foundation.Domain.Events;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.SeedWork;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : IEquatable<TId>, IId<TId>
{
    protected AggregateRoot()
    {
        _events = new HashSet<DomainEvent>();
    }

    private readonly ICollection<DomainEvent> _events;

    public IReadOnlyCollection<DomainEvent> Events => [.. _events];

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void RaiseDomainEvent(DomainEvent @event)
    {
        _events.Add(@event);
    }
}
