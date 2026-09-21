namespace Andor.Foundation.Infrastructure.Messaging;

/// <summary>
/// Configuration options for publishing messages to an Azure Service Bus topic.
/// </summary>
public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    /// <summary>
    /// The fully qualified Service Bus namespace, e.g. "my-namespace.servicebus.windows.net".
    /// Used together with Managed Identity / DefaultAzureCredential (recommended).
    /// </summary>
    public string? FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// Optional connection string. Prefer <see cref="FullyQualifiedNamespace"/> with Managed Identity
    /// instead of connection strings whenever possible.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// The default topic name that messages are published to.
    /// </summary>
    public string TopicName { get; set; } = string.Empty;

    /// <summary>
    /// Optional queue name for point-to-point sends (<see cref="Application.IMessageSenderInterface.QueueSendAsync"/>).
    /// Leave empty for modules that only publish to <see cref="TopicName"/>.
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    /// Additional named queues, each with its own credential — for point-to-point sends where the
    /// receiver should only be able to grant the sender a Send-only SAS scoped to that one queue,
    /// instead of every sender in the process sharing one broad connection string/credential.
    /// Keyed by an arbitrary logical name passed to
    /// <see cref="Application.IMessageSenderInterface.QueueSendAsync(string, object, string, System.Threading.CancellationToken)"/>.
    /// </summary>
    public Dictionary<string, NamedQueueOptions> Queues { get; set; } = new();
}

/// <summary>
/// Connection + destination for one entry in <see cref="ServiceBusOptions.Queues"/>. Mirrors
/// <see cref="ServiceBusOptions"/>'s own namespace/connection-string/name shape, but scoped to a
/// single queue so it can carry its own (narrower) credential.
/// </summary>
public sealed class NamedQueueOptions
{
    public string? FullyQualifiedNamespace { get; set; }

    public string? ConnectionString { get; set; }

    public string QueueName { get; set; } = string.Empty;
}
