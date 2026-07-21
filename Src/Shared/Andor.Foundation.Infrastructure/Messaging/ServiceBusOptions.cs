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
}
