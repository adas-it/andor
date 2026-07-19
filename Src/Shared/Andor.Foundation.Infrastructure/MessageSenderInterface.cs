using System.Text.Json;
using Andor.Foundation.Application;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Andor.Foundation.Infrastructure;

/// <summary>
/// Publishes messages to an Azure Service Bus topic using the native
/// <see cref="Azure.Messaging.ServiceBus"/> SDK (no MassTransit).
/// </summary>
internal sealed class MessageSenderAzure : IMessageSenderInterface
{
    private readonly ServiceBusSender _sender;
    private readonly ILogger<MessageSenderAzure> _logger;

    public MessageSenderAzure(
        ServiceBusSender sender,
        ILogger<MessageSenderAzure> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task PubSubSendAsync(object data, string messageId, CancellationToken cancellationToken)
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
            await _sender.SendMessageAsync(message, cancellationToken);

            _logger.LogInformation(
                "Published message {MessageType} with id {MessageId} to topic {Topic}.",
                messageType.FullName,
                message.MessageId,
                _sender.EntityPath);
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish message {MessageType} to topic {Topic}. Reason: {Reason}.",
                messageType.FullName,
                _sender.EntityPath,
                ex.Reason);

            throw;
        }
    }
}

