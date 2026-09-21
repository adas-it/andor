using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Andor.Users.WebApi.Consumers;

internal sealed record IdentityProvisioningRequestedMessage(Guid UserId, string Email, string Name, string PasswordHash);

/// <summary>
/// Consumes the dedicated "request-identity-user" queue — published by
/// <c>Andor.Users.Service</c> right after it provisions a User — and creates the corresponding
/// credentials row. The password arrives already hashed; Onboarding never puts a raw password on
/// the bus. Unlike the old "user-verified-events" topic subscription this replaces, this queue is
/// single-purpose, so there's no envelope/event-name gate to check before deserializing.
/// </summary>
public sealed class IdentityProvisioningConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IdentityProvisioningConsumer> _logger;

    public IdentityProvisioningConsumer(
        ServiceBusClient client,
        IOptions<IdentityProvisioningQueueOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<IdentityProvisioningConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.QueueName))
        {
            throw new InvalidOperationException("IdentityProvisioningQueue:QueueName must be configured.");
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
        var message = args.Message.Body.ToObjectFromJson<IdentityProvisioningRequestedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exists = await db.Users.AnyAsync(u => u.Id == message.UserId, args.CancellationToken);

        if (!exists)
        {
            _ = db.Users.Add(new ApplicationUser
            {
                Id = message.UserId,
                UserName = message.Email,
                PasswordHash = message.PasswordHash,
                Group = "User",
            });

            _ = await db.SaveChangesAsync(args.CancellationToken);
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing identity provisioning message.");
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

/// <summary>
/// Deliberately its own connection, separate from any other Service Bus config in this app: this
/// queue is meant to carry a Listen-only SAS scoped to just "request-identity-user", not the
/// broader credential other parts of the app might use.
/// </summary>
public sealed class IdentityProvisioningQueueOptions
{
    public const string SectionName = "IdentityProvisioningQueue";

    public string? FullyQualifiedNamespace { get; set; }
    public string? ConnectionString { get; set; }
    public string QueueName { get; set; } = string.Empty;
}
