using Andor.Foundation.Application;
using Andor.Foundation.Domain.Events;
using Andor.Users.Contracts.Requests;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Onboarding.Service.Consumers;

internal sealed record SignupVerifiedForProvisioningMessage(
    Guid UserId,
    string Name,
    string Email,
    string PasswordHash,
    Guid PreferredLanguageId,
    Guid PreferredCurrencyId,
    bool MarketingOptIn,
    bool TermsAndConditionsAccepted,
    bool PrivacyPolicyAccepted);

/// <summary>
/// Subscribes to the "user-verified-events" topic and, upon receiving a
/// <see cref="SignupVerifiedDomainEvent"/>, forwards it as a point-to-point message on the
/// "request-user-provisioning" queue so Users.Service can create the User (and, from there,
/// Identity credentials and a default Account) without Onboarding taking a hard, synchronous
/// dependency on that chain.
/// </summary>
public sealed class UserProvisioningRequestConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserProvisioningRequestConsumer> _logger;

    public UserProvisioningRequestConsumer(
        ServiceBusClient client,
        IOptions<UserProvisioningRequestSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<UserProvisioningRequestConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "UserProvisioningRequestSubscription:TopicName and SubscriptionName must be configured.");
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

        var message = args.Message.Body.ToObjectFromJson<SignupVerifiedForProvisioningMessage>();

        using var scope = _scopeFactory.CreateScope();
        var messageSender = scope.ServiceProvider.GetRequiredService<IMessageSenderInterface>();

        await messageSender.QueueSendAsync(
            "UserProvisioning",
            new UserProvisioningRequestMessage(
                message.UserId,
                message.Name,
                message.Email,
                message.PasswordHash,
                message.PreferredLanguageId,
                message.PreferredCurrencyId,
                message.MarketingOptIn,
                message.TermsAndConditionsAccepted,
                message.PrivacyPolicyAccepted),
            $"user-provisioning-{message.UserId:N}",
            args.CancellationToken);

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing user-verified message for provisioning.");
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
