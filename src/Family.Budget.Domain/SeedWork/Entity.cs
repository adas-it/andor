namespace Family.Budget.Domain.SeedWork;

using Family.Budget.Domain.Common;
using Family.Budget.Domain.Exceptions;
using Family.Budget.Domain.Validation;
using System.Collections.Immutable;

public abstract class Entity : IAggregateRoot
{
    public Guid Id { get; init; }
    protected ICollection<Notification> Notifications { get; private set; }

    protected Entity()
    {
        Notifications = new HashSet<Notification>();
        _events = new HashSet<DomainEventBase>();
    }

    protected virtual void Validate()
    {
        AddNotification(Id!.NotNull());

        if (Notifications.Any())
        {
            throw new InvalidDomainException(Notifications.ToList().GetMessage(), CommonErrorCodes.Validation);
        }
    }

    protected void AddNotification(Notification? notification)
    {
        if (notification != null)
        {
            Notifications.Add(notification);
        }
    }

    private readonly ICollection<DomainEventBase> _events;

    public IReadOnlyCollection<DomainEventBase> Events => _events.ToImmutableArray();

    public void ClearEvents()
    {
        _events.Clear();
    }

    internal void RaiseDomainEvent(DomainEventBase @event)
    {
        _events.Add(@event);
    }
}