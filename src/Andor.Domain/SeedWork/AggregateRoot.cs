using System.Collections.Immutable;

namespace Andor.Domain.SeedWork;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot where T : IEquatable<T>
{
    protected AggregateRoot()
    {
        _events = new HashSet<object>();
    }


    private readonly ICollection<object> _events;

    public IReadOnlyCollection<object> Events => _events.ToImmutableArray();

    public void ClearEvents()
    {
        _events.Clear();
    }

    internal void RaiseDomainEvent(object @event)
    {
        _events.Add(@event);
    }
}
