using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Andor.Users.WebApi.Consumers;

internal sealed record UserVerifiedMessage(Guid UserId, string Name, string Email, string PasswordHash);

/// <summary>
/// Consumes the "user-verified-events" topic (published by the Onboarding module once a
/// signup is confirmed) and creates the corresponding credentials row. The password arrives
/// already hashed — Onboarding never puts a raw password on the bus.
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
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exists = await db.Users.AnyAsync(u => u.Id == message.UserId, args.CancellationToken);

        if (!exists)
        {
            db.Users.Add(new ApplicationUser
            {
                Id = message.UserId,
                UserName = message.Email,
                PasswordHash = message.PasswordHash,
                Group = "User",
            });

            await db.SaveChangesAsync(args.CancellationToken);
        }

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
