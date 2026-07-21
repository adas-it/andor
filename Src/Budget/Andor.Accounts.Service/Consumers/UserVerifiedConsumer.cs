using Andor.Accounts.Application.Commands;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Authorizations.Domain;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Accounts.Service.Consumers;

internal sealed record UserVerifiedMessage(Guid UserId, string Name, string Email);

public sealed class SignupVerifiedSubscriptionOptions
{
    public const string SectionName = "SignupVerifiedSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes the "user-verified-events" topic (published by the Onboarding module once a
/// signup is confirmed) and auto-creates a personal Account for the newly verified user.
/// </summary>
public sealed class UserVerifiedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserVerifiedConsumer> _logger;

    public UserVerifiedConsumer(
        ServiceBusClient client,
        IOptions<SignupVerifiedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<UserVerifiedConsumer> logger)
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
        var message = args.Message.Body.ToObjectFromJson<UserVerifiedMessage>();

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

        await commandsService.CreateAccountAsync(command);

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
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
