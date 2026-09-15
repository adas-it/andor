using Andor.Foundation.Domain.Events;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Application.Commands;
using Andor.Users.Application.Interfaces;
using Andor.Users.Domain.Users.ValueObjects;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Users.Service.Consumers;

internal sealed record UserVerifiedMessage(Guid UserId, string Name, string Email);

/// <summary>
/// Consumes the "user-verified-events" topic (published by the Onboarding module once a signup
/// is confirmed) and provisions the module's User aggregate, which holds the user's preferences.
/// </summary>
public sealed class UserVerifiedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserVerifiedConsumer> _logger;

    public UserVerifiedConsumer(
        ServiceBusClient client,
        IOptions<UserVerifiedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<UserVerifiedConsumer> logger)
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

        if (domainEvent.EventName != "SignupVerifiedDomainEvent")
        {
            _logger.LogDebug("Received unexpected event type: {EventType}.", domainEvent.EventName);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }
        ;

        var message = args.Message.Body.ToObjectFromJson<UserVerifiedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var commandsService = scope.ServiceProvider.GetRequiredService<IUserCommandsService>();

        var (firstName, lastName) = SplitName(message.Name);

        var command = new CreateUserCommand(
            UserId.Load(message.UserId),
            new Email(message.Email),
            firstName,
            lastName,
            Guid.Empty,
            Guid.Empty,
            args.CancellationToken);

        var result = await commandsService.CreateUserAsync(command);

        if (result.IsFailure)
        {
            _logger.LogError(
                "Failed to provision user preferences for user {UserId}.", message.UserId);
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private static (string FirstName, string LastName) SplitName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (name, name);
        }

        var parts = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], parts[0]);
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
