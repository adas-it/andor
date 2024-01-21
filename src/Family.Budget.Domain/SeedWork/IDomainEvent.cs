namespace Family.Budget.Domain.SeedWork;
using MediatR;
public abstract record DomainEventBase : INotification
{
    public Guid EventId { get; }
    public DateTime EventDateUtc { get; }
    public string EventName { get; set; }

    public DomainEventBase(string eventName)
    {
        EventId = Guid.NewGuid();
        EventDateUtc = DateTime.UtcNow;
        EventName = eventName;
    }
}
