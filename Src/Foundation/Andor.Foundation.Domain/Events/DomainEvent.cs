namespace Andor.Foundation.Domain.Events;

public record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime EventDate { get; init; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public Guid Id { get; init; }
}
