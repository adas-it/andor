using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Authorizations.Domain;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Andor.Accounts.Service.Consumers;

internal sealed record AccountProvisioningRequestedMessage(Guid UserId, string Email, string Name);

/// <summary>
/// Deliberately its own connection, separate from any other Service Bus config in this app: this
/// queue is meant to carry a Listen-only SAS scoped to just "request-account-creation", not the
/// broader credential this service's own event publishing uses.
/// </summary>
public sealed class AccountProvisioningQueueOptions
{
    public const string SectionName = "AccountProvisioningQueue";

    public string? FullyQualifiedNamespace { get; set; }
    public string? ConnectionString { get; set; }
    public string QueueName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes the dedicated "request-account-creation" queue — published by
/// <c>Andor.Users.Service</c> right after it provisions a User — and auto-creates a personal
/// Account for the newly verified user. Replaces the old "user-verified-events" topic
/// subscription: this queue is single-purpose, so there's no envelope/event-name gate to check
/// before deserializing.
/// </summary>
public sealed class AccountProvisioningConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountProvisioningConsumer> _logger;

    public const string ClientKey = "AccountProvisioningQueue";

    public AccountProvisioningConsumer(
        [FromKeyedServices(ClientKey)] ServiceBusClient client,
        IOptions<AccountProvisioningQueueOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<AccountProvisioningConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.QueueName))
        {
            throw new InvalidOperationException("AccountProvisioningQueue:QueueName must be configured.");
        }

        _processor = client.CreateProcessor(options.Value.QueueName);
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
        var message = args.Message.Body.ToObjectFromJson<AccountProvisioningRequestedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var currencyRepository = scope.ServiceProvider.GetRequiredService<ICommandsCurrencyRepository>();
        var commandsService = scope.ServiceProvider.GetRequiredService<IAccountCommandsService>();

        var currency = await currencyRepository.GetByIsoAsync("BRL", args.CancellationToken);

        if (currency == null)
        {
            _logger.LogError(
                "Default currency 'BRL' has not been seeded yet; cannot auto-create an account for user {UserId}.",
                message.UserId);
            return;
        }

        var currentUser = new ApplicationUser(message.UserId, "User", true, "TenantA");

        var command = new CreateAccountCommand(
            AccountId.New(),
            $"Conta de {message.Name}",
            "Conta pessoal criada automaticamente no cadastro",
            currency.Id,
            currentUser,
            args.CancellationToken);

        _ = await commandsService.CreateAccountAsync(command);

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing account provisioning message.");
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
