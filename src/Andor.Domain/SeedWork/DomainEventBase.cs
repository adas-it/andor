namespace Andor.Domain.SeedWork;

public interface IDomainEventBase
{
    string EventName { get; init; }
    DateTime EventDateUTC { get; init; }
}

public abstract record DomainEventBase<T> : IDomainEventBase where T : IAggregateRoot
{
    protected DomainEventBase(string eventName, T context)
    {
        EventName = eventName;
        Context = context;
    }

    public string EventName { get; init; }
    public DateTime EventDateUTC { get; init; } = DateTime.UtcNow;
    public T Context { get; set; }
}

