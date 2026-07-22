using Andor.Foundation.Application;

namespace Andor.ComponentTests.Common;

/// <summary>
/// Replaces the real Azure Service Bus sender in component tests. Without this, any aggregate
/// write that enqueues an Outbox message would have the Outbox dispatcher (or a queue consumer)
/// try to reach a real Azure namespace using the fake connection strings checked into
/// appsettings.
/// </summary>
public sealed class NullMessageSender : IMessageSenderInterface
{
    public Task PubSubSendAsync(object data, string messageId, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task QueueSendAsync(object data, string messageId, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
