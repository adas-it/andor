namespace Andor.Foundation.Application;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, string messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a point-to-point message to the module's configured queue.
    /// Throws <see cref="InvalidOperationException"/> if the module didn't configure a queue.
    /// </summary>
    Task QueueSendAsync(object data, string messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a point-to-point message to one of the module's named additional queues
    /// (<c>ServiceBus:Queues:{queueKey}</c>), each of which can carry its own scoped credential.
    /// Throws <see cref="InvalidOperationException"/> if no queue was configured under that key.
    /// </summary>
    Task QueueSendAsync(string queueKey, object data, string messageId, CancellationToken cancellationToken);
}
