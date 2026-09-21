using Andor.Communications.Contracts.Requests;
using Andor.Foundation.Application;
using Andor.Foundation.Domain.Events;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Onboarding.Service.Consumers;

internal sealed record SignupVerifiedMessage(Guid UserId, string Name, string Email, string PreferredLanguage);

/// <summary>
/// Subscribes to the "user-verified-events" topic and, upon receiving a
/// <see cref="SignupVerifiedDomainEvent"/>, publishes a <see cref="SendNotificationInput"/> to the
/// Communications module's "request-communication" queue so a welcome email can be sent to the
/// newly verified user.
/// </summary>
public sealed class SignupVerifiedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SignupVerifiedConsumer> _logger;

    public SignupVerifiedConsumer(
        ServiceBusClient client,
        IOptions<SignupVerifiedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<SignupVerifiedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "SignupVerifiedSubscription:TopicName and SubscriptionName must be configured.");
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

        if (domainEvent.EventName != "SignupVerifiedDomainEvent")
        {
            _logger.LogDebug("Received unexpected event type: {EventType}.", domainEvent.EventName);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }
        ;

        var message = args.Message.Body.ToObjectFromJson<SignupVerifiedMessage>();

        var notification = new SendNotificationInput(
            RuleId: Guid.Parse("875725eb-683a-4f33-b27f-32489d127e4b"),
            RecipientEmail: message.Email,
            TemplateTitle: "wellcome",
            ContentLanguage: string.IsNullOrWhiteSpace(message.PreferredLanguage) ? "en" : message.PreferredLanguage,
            Values: new Dictionary<string, string>
            {
                { "<name>", message.Name }
            });

        using var scope = _scopeFactory.CreateScope();
        var messageSender = scope.ServiceProvider.GetRequiredService<IMessageSenderInterface>();

        try
        {
            await messageSender.QueueSendAsync(notification, args.Message.MessageId, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish welcome communication request for user {UserId}.", message.UserId);

            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing user-verified message.");
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
