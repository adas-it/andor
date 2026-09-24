using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Application.Commands;
using Andor.Users.Application.Interfaces;
using Andor.Users.Contracts.Requests;
using Andor.Users.Domain.Users.ValueObjects;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Users.Service.Consumers;

/// <summary>
/// Consumes the dedicated "request-user-provisioning" queue — published by Onboarding.Service
/// once a signup is verified — and starts the User/Identity/Account provisioning chain that used
/// to run synchronously inside <c>POST /v1/users</c>. This queue is single-purpose, so there's no
/// envelope/event-name gate to check before deserializing.
/// </summary>
public sealed class UserProvisioningRequestedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserProvisioningRequestedConsumer> _logger;

    public const string ClientKey = "UserProvisioningQueue";

    public UserProvisioningRequestedConsumer(
        [FromKeyedServices(ClientKey)] ServiceBusClient client,
        IOptions<UserProvisioningQueueOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<UserProvisioningRequestedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.QueueName))
        {
            throw new InvalidOperationException("UserProvisioningQueue:QueueName must be configured.");
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
        var message = args.Message.Body.ToObjectFromJson<UserProvisioningRequestMessage>();

        using var scope = _scopeFactory.CreateScope();
        var userCommands = scope.ServiceProvider.GetRequiredService<IUserCommandsService>();

        var (firstName, lastName) = SplitName(message.Name);

        var command = new CreateUserCommand(
            UserId.Load(message.UserId),
            new Email(message.Email),
            firstName,
            lastName,
            message.PreferredLanguageId,
            message.PreferredCurrencyId,
            message.MarketingOptIn,
            message.TermsAndConditionsAccepted,
            message.PrivacyPolicyAccepted,
            message.PasswordHash,
            args.CancellationToken);

        var result = await userCommands.CreateUserAsync(command);

        if (result.IsSuccess)
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        else
        {
            _logger.LogError("Failed to provision user {UserId}: {Errors}.",
                message.UserId, string.Join(", ", result.Errors.Select(e => e.Message)));

            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing user provisioning message.");
        return Task.CompletedTask;
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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Let ExecuteAsync's finally block call StopProcessingAsync on a still-live processor
        // before we dispose it; disposing first races the message pump and disposes its
        // internal SemaphoreSlim out from under it, crashing the host on shutdown.
        await base.StopAsync(cancellationToken);
        await _processor.DisposeAsync();
    }
}

/// <summary>
/// Deliberately its own connection, separate from the shared ServiceBus config this app publishes
/// its own events through: this queue is meant to carry a Listen-only SAS scoped to just
/// "request-user-provisioning".
/// </summary>
public sealed class UserProvisioningQueueOptions
{
    public const string SectionName = "UserProvisioningQueue";

    public string? FullyQualifiedNamespace { get; set; }
    public string? ConnectionString { get; set; }
    public string QueueName { get; set; } = string.Empty;
}
