using Andor.Foundation.Domain.Events;
using Andor.Onboarding.Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Onboarding.Service.Consumers;

internal sealed record UserCreatedMessage(Guid UserId);

/// <summary>
/// Subscribes to the Users module's "andor-users-events" topic and, upon receiving a
/// UserCreatedDomainEvent, requests the welcome email by publishing onto Communications'
/// "request-communication" queue — passing only the UserId, since the consumer on the other end
/// enriches Email/PreferredLanguage/"&lt;name&gt;" from its own Recipient projection instead of
/// needing them resent here. Triggering on User creation (not on signup verification) means the
/// welcome only goes out once the User actually exists.
/// </summary>
public sealed class UserCreatedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(
        ServiceBusClient client,
        IOptions<UserCreatedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<UserCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "UserCreatedSubscription:TopicName and SubscriptionName must be configured.");
        }

        _processor = client.CreateProcessor(options.Value.TopicName, options.Value.SubscriptionName);
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await _processor.StopProcessingAsync(CancellationToken.None);
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var domainEvent = args.Message.Body.ToObjectFromJson<DomainEvent>();

        if (domainEvent.EventName != "UserCreatedDomainEvent")
        {
            _logger.LogDebug("Received unexpected event type: {EventType}.", domainEvent.EventName);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        var message = args.Message.Body.ToObjectFromJson<UserCreatedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var communicationRequestClient = scope.ServiceProvider.GetRequiredService<ICommunicationRequestClient>();

        var result = await communicationRequestClient.RequestAsync(
            ruleId: Guid.Parse("875725eb-683a-4f33-b27f-32489d127e4b"),
            templateTitle: "wellcome",
            userId: message.UserId,
            recipientEmail: null,
            contentLanguage: null,
            values: null,
            args.CancellationToken);

        if (result.IsSuccess)
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        else
        {
            _logger.LogError("Failed to request welcome communication for user {UserId}: {Errors}.",
                message.UserId, string.Join(", ", result.Errors.Select(e => e.Message)));

            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing user-created message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Let ExecuteAsync's finally block call StopProcessingAsync on a still-live processor
        // before we dispose it; disposing first races the message pump and disposes its
        // internal SemaphoreSlim out from under it, crashing the host on shutdown.
        await base.StopAsync(cancellationToken);
        await _processor.DisposeAsync();
    }
}
