using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.Events;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Communications.Service.Consumers;

internal sealed record SignupVerifiedMessage(
    Guid UserId, string Name, string Email, string PreferredLanguage,
    bool MarketingOptIn, bool TermsAndConditionsAccepted, bool PrivacyPolicyAccepted);

public sealed class RecipientSyncSubscriptionOptions
{
    public const string SectionName = "RecipientSyncSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes the "user-verified-events" topic (published by Onboarding once a signup is
/// confirmed) and keeps the local Recipient projection in sync, so requesting a communication
/// doesn't need Name/Email/PreferredLanguage/consent resent on every call.
/// </summary>
public sealed class RecipientSyncConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecipientSyncConsumer> _logger;

    public RecipientSyncConsumer(
        ServiceBusClient client,
        IOptions<RecipientSyncSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RecipientSyncConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "RecipientSyncSubscription:TopicName and SubscriptionName must be configured.");
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

        var message = args.Message.Body.ToObjectFromJson<SignupVerifiedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandsRecipientRepository>();

        var recipientId = RecipientId.Load(message.UserId);
        var existing = await repository.GetByIdAsync(recipientId, args.CancellationToken);

        if (existing is not null)
        {
            var updateResult = existing.Update(message.Name, message.Email, message.PreferredLanguage, active: true,
                message.MarketingOptIn, message.TermsAndConditionsAccepted, message.PrivacyPolicyAccepted);

            if (updateResult.IsSuccess)
            {
                await repository.PersistAsync(existing, args.CancellationToken);
            }
        }
        else
        {
            var (result, recipient) = Recipient.New(recipientId, message.Name, message.Email, message.PreferredLanguage,
                active: true, message.MarketingOptIn, message.TermsAndConditionsAccepted, message.PrivacyPolicyAccepted);

            if (result.IsSuccess && recipient is not null)
            {
                await repository.PersistAsync(recipient, args.CancellationToken);
            }
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing recipient sync message.");
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
