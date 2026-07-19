using System.Text.Json;
using Andor.Foundation.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Andor.Foundation.Infrastructure.Outbox;

/// <summary>
/// Generic background dispatcher that implements the "relay" side of the transactional
/// Outbox pattern. It polls every Outbox source exposed by the registered
/// <see cref="IOutboxContextProvider"/> instances and publishes pending messages to the
/// broker via <see cref="IMessageSenderInterface"/>. Being module-agnostic, it lives in
/// Foundation and can be reused by any module that persists <see cref="OutboxMessage"/>.
/// </summary>
public sealed class OutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxDispatcher> logger) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;
    private const int MaxAttempts = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox dispatcher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAllSourcesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during outbox dispatch cycle.");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("Outbox dispatcher stopping.");
    }

    private async Task ProcessAllSourcesAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var providers = scope.ServiceProvider.GetServices<IOutboxContextProvider>();
        var sender = scope.ServiceProvider.GetRequiredService<IMessageSenderInterface>();

        foreach (var provider in providers)
        {
            foreach (var context in provider.CreateContexts())
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using (context)
                {
                    await ProcessContextAsync(context, sender, cancellationToken);
                }
            }
        }
    }

    private async Task ProcessContextAsync(
        PrincipalContext context,
        IMessageSenderInterface sender,
        CancellationToken cancellationToken)
    {
        var pending = await context.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.Attempts < MaxAttempts)
            .OrderBy(m => m.OccurredOn)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (pending.Count == 0)
        {
            return;
        }

        foreach (var message in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var eventType = Type.GetType(message.Type)
                    ?? throw new InvalidOperationException(
                        $"Could not resolve outbox message type '{message.Type}'.");

                var payload = JsonSerializer.Deserialize(message.Content, eventType)
                    ?? throw new InvalidOperationException(
                        $"Could not deserialize outbox message '{message.Id}'.");

                await sender.PubSubSendAsync(payload, message.Id.ToString("N"), cancellationToken);

                message.ProcessedOn = DateTimeOffset.UtcNow;
                message.Error = null;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                message.Attempts++;
                message.Error = ex.Message;

                logger.LogError(
                    ex,
                    "Failed to dispatch outbox message {MessageId} (attempt {Attempts}).",
                    message.Id,
                    message.Attempts);
            }
        }

        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
