namespace Andor.Foundation.Domain.Events;

/// <summary>
/// Opts a <see cref="DomainEvent"/> into point-to-point queue delivery instead of the default
/// topic broadcast when the Outbox dispatches it. Domain events that don't implement this are
/// unaffected and keep going to the module's topic exactly as before.
/// </summary>
public interface IQueueRoutedDomainEvent
{
    string QueueName { get; }
}
