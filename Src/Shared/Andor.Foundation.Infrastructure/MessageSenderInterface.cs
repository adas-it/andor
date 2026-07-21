using System.Text.Json;
using Andor.Foundation.Application;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Andor.Foundation.Infrastructure;

/// <summary>
/// Publishes messages to an Azure Service Bus topic and/or queue using the native
/// <see cref="Azure.Messaging.ServiceBus"/> SDK (no MassTransit).
/// </summary>
internal sealed class MessageSenderAzure : IMessageSenderInterface
{
    private readonly ServiceBusSenders _senders;
    private readonly ILogger<MessageSenderAzure> _logger;

    public MessageSenderAzure(
        ServiceBusSenders senders,
        ILogger<MessageSenderAzure> logger)
    {
        _senders = senders;
        _logger = logger;
    }

    public Task PubSubSendAsync(object data, string messageId, CancellationToken cancellationToken)
        => SendAsync(_senders.Topic, data, messageId, cancellationToken);

    public Task QueueSendAsync(object data, string messageId, CancellationToken cancellationToken)
    {
        if (_senders.Queue is null)
        {
            throw new InvalidOperationException(
                "No queue was configured for this module (ServiceBus:QueueName is empty).");
        }

        return SendAsync(_senders.Queue, data, messageId, cancellationToken);
    }

    private async Task SendAsync(ServiceBusSender sender, object data, string messageId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);

        var messageType = data.GetType();

        // Serialize the payload. The concrete runtime type is preserved through the
        // ApplicationProperties/Subject so subscribers can deserialize correctly.
        var body = JsonSerializer.SerializeToUtf8Bytes(data, messageType);

        var message = new ServiceBusMessage(body)
        {
            ContentType = "application/json",
            Subject = messageType.Name,
            // Stable MessageId enables end-to-end idempotency / duplicate detection
            // when the caller supplies a deterministic identifier (e.g. the Outbox row id).
            MessageId = messageId,
        };

        message.ApplicationProperties["MessageType"] = messageType.FullName;

        try
        {
            // ServiceBusClient/Sender already applies exponential-backoff retries
            // for transient failures based on the configured ServiceBusRetryOptions.
            await sender.SendMessageAsync(message, cancellationToken);

            _logger.LogInformation(
                "Published message {MessageType} with id {MessageId} to {EntityPath}.",
                messageType.FullName,
                message.MessageId,
                sender.EntityPath);
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish message {MessageType} to {EntityPath}. Reason: {Reason}.",
                messageType.FullName,
                sender.EntityPath,
                ex.Reason);

            throw;
        }
    }
}

/// <summary>
/// Holds the module's Service Bus senders. <see cref="Queue"/> is null when the module didn't
/// configure <see cref="Messaging.ServiceBusOptions.QueueName"/>. Registered as a single object
/// so both senders can be resolved from DI without ambiguous <see cref="ServiceBusSender"/> registrations.
/// </summary>
internal sealed class ServiceBusSenders
{
    public required ServiceBusSender Topic { get; init; }
    public ServiceBusSender? Queue { get; init; }
}
