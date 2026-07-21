using Andor.Application.Communications.Interfaces;
using Andor.Foundation.Infrastructure.Messaging;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Andor.Communications.Service.Consumers;

internal sealed record SignupCodeMessage(string Name, string Email, string Code);

/// <summary>
/// Consumes the "signup-verification-codes" queue (populated by the Onboarding module's Outbox)
/// and sends the code by e-mail. Bypasses the Rule/Template aggregate entirely — a raw ISMTP
/// send is enough for a one-off verification code and doesn't require a pre-created Rule.
/// </summary>
public sealed class SignupCodeQueueConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SignupCodeQueueConsumer> _logger;

    public SignupCodeQueueConsumer(
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<SignupCodeQueueConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var queueName = options.Value.QueueName;

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException(
                "ServiceBus:QueueName must be configured for Communications to consume signup verification codes.");
        }

        _processor = client.CreateProcessor(queueName);
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
        var message = args.Message.Body.ToObjectFromJson<SignupCodeMessage>();

        using var scope = _scopeFactory.CreateScope();
        var smtp = scope.ServiceProvider.GetRequiredService<ISMTP>();

        var body = $"Ola {message.Name},<br/>Seu codigo de verificacao e: <strong>{message.Code}</strong><br/>Ele expira em 15 minutos.";

        await smtp.Handler(message.Email, body, "Seu codigo de verificacao", args.CancellationToken);

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing signup verification code message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
