using System.Collections.Immutable;

namespace Andor.Domain.SeedWork;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot where T : IEquatable<T>
{
    protected AggregateRoot()
    {
        _events = new HashSet<IDomainEventBase>();
    }


    private readonly ICollection<IDomainEventBase> _events;

    public IReadOnlyCollection<IDomainEventBase> Events => _events.ToImmutableArray();

    public void ClearEvents()
    {
        _events.Clear();
    }

    internal void RaiseDomainEvent(IDomainEventBase @event)
    {
        _events.Add(@event);
    }
}
