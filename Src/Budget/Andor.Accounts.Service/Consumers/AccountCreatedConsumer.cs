using Andor.Accounts.Application.Commands;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Authorizations.Domain;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Accounts.Service.Consumers;

internal sealed record AccountCreatedMessage(Guid Id, string Name, Guid UserId);

public sealed class AccountCreatedSubscriptionOptions
{
    public const string SectionName = "AccountCreatedSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes this module's own "andor-accounts-events" topic and, once a new Account is
/// created, seeds it with the current default Category/SubCategory/PaymentMethod templates.
/// The topic carries every Account* domain event, so non-matching messages are completed
/// (not processed) rather than filtered server-side, since no subscription filter rule is
/// provisioned for this in the repo.
/// </summary>
public sealed class AccountCreatedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountCreatedConsumer> _logger;

    public AccountCreatedConsumer(
        ServiceBusClient client,
        IOptions<AccountCreatedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<AccountCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "AccountCreatedSubscription:TopicName and SubscriptionName must be configured.");
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
        if (args.Message.Subject != nameof(AccountCreatedDomainEvent))
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        var message = args.Message.Body.ToObjectFromJson<AccountCreatedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var commandsService = scope.ServiceProvider.GetRequiredService<IAccountCommandsService>();

        var currentUser = new ApplicationUser(message.UserId, "User", true, "TenantA");

        var command = new SeedAccountDefaultsCommand(
            AccountId.Load(message.Id),
            currentUser,
            args.CancellationToken);
        try
        {
            await commandsService.SeedAccountDefaultsAsync(command);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding account defaults for account {AccountId}.", message.Id);
        }
        finally
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing account-created message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
