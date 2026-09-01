using Andor.Communications.Contracts.Requests;
using Andor.Foundation.Application;
using Andor.Foundation.Domain.Events;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Onboarding.Service.Consumers;

internal sealed record UserVerifiedMessage(Guid UserId, string Name, string Email, string Code);

/// <summary>
/// Subscribes to the "user-verified-events" topic and, upon receiving a message, publishes a
/// <see cref="SendNotificationInput"/> to the Communications module's "request-communication"
/// queue so a notification can be sent to the verified user.
/// </summary>
public sealed class SignupCodeGeneratedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SignupCodeGeneratedConsumer> _logger;

    public SignupCodeGeneratedConsumer(
        ServiceBusClient client,
        IOptions<UserVerifiedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<SignupCodeGeneratedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "UserVerifiedSubscription:TopicName and SubscriptionName must be configured.");
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

        if (domainEvent.EventName != "SignupCodeGenerated")
        {
            _logger.LogDebug("Received unexpected event type: {EventType}.", domainEvent.EventName);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }
        ;

        var message = args.Message.Body.ToObjectFromJson<UserVerifiedMessage>();

        var notification = new SendNotificationInput(
            RuleId: Guid.Parse("acb860a5-1af6-4b03-afae-e290dfcac7d4"),
            RecipientEmail: message.Email,
            TemplateTitle: "wellcome",
            ContentLanguage: "en",
            Values: new Dictionary<string, string>
            {
                { "<code>", message.Code },
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
            _logger.LogError(ex, "Failed to publish communication request for user {UserId}.", message.UserId);

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
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
