namespace Andor.Foundation.Infrastructure.Outbox;

/// <summary>
/// Transactional Outbox record. A row is written in the SAME transaction as the
/// aggregate that raised the domain event, guaranteeing atomicity between the
/// business change and the intent to publish a message. A background dispatcher
/// later reads unprocessed rows and publishes them to the message broker.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>Unique identifier of the outbox record (also used as broker MessageId for idempotency).</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Assembly-qualified type name of the serialized event, used to rehydrate the payload.</summary>
    public string Type { get; set; } = default!;

    /// <summary>JSON serialized payload of the domain event.</summary>
    public string Content { get; set; } = default!;

    /// <summary>When the event occurred / was enqueued (UTC).</summary>
    public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>When the message was successfully published (UTC). Null while pending.</summary>
    public DateTimeOffset? ProcessedOn { get; set; }

    /// <summary>Number of publish attempts made so far.</summary>
    public int Attempts { get; set; }

    /// <summary>Last error message captured on a failed publish attempt.</summary>
    public string? Error { get; set; }

    /// <summary>
    /// When set, the dispatcher sends this message to the named queue instead of the module's
    /// topic (see <see cref="Andor.Foundation.Domain.Events.IQueueRoutedDomainEvent"/>). Null for
    /// the default, topic-broadcast behavior.
    /// </summary>
    public string? TargetQueue { get; set; }
}
