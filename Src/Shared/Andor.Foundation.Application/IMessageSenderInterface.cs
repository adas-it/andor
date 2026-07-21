namespace Andor.Foundation.Application;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, string messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a point-to-point message to the module's configured queue.
    /// Throws <see cref="InvalidOperationException"/> if the module didn't configure a queue.
    /// </summary>
    Task QueueSendAsync(object data, string messageId, CancellationToken cancellationToken);
}
