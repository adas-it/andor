using Andor.Foundation.Domain.Events;
using Andor.Onboarding.Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Onboarding.Service.Consumers;

internal sealed record UserVerifiedMessage(Guid UserId, string Name, string Email, string Code, string PreferredLanguage);

/// <summary>
/// Subscribes to the "user-verified-events" topic and, upon receiving a message, requests the
/// verification-code email via Communications.Service's POST /v1/communications/requests. No
/// User exists yet at this point in the flow, so this passes RecipientEmail (not UserId) — the
/// one case the enrichment/consent path in that endpoint doesn't apply to.
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

        using var scope = _scopeFactory.CreateScope();
        var communicationRequestClient = scope.ServiceProvider.GetRequiredService<ICommunicationRequestClient>();

        var result = await communicationRequestClient.RequestAsync(
            ruleId: Guid.Parse("acb860a5-1af6-4b03-afae-e290dfcac7d4"),
            templateTitle: "wellcome",
            userId: null,
            recipientEmail: message.Email,
            contentLanguage: string.IsNullOrWhiteSpace(message.PreferredLanguage) ? "en" : message.PreferredLanguage,
            values: new Dictionary<string, string>
            {
                { "<code>", message.Code },
                { "<name>", message.Name },
            },
            args.CancellationToken);

        if (result.IsSuccess)
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        else
        {
            _logger.LogError("Failed to request communication for user {UserId}: {Errors}.",
                message.UserId, string.Join(", ", result.Errors.Select(e => e.Message)));

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
