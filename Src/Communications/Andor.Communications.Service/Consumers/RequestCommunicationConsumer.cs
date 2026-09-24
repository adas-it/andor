using Andor.Communications.Application.Interfaces;
using Andor.Communications.Contracts.Requests;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Communications.Service.Consumers;

public sealed class RequestCommunicationQueueOptions
{
    public const string SectionName = "RequestCommunicationQueue";

    public string QueueName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes the "request-communication" queue (published by Onboarding, and any other module that
/// wants a communication sent) and runs it through <see cref="IRequestCommunicationService"/>: the
/// Recipient enrichment and Marketing-consent gate, before republishing onto "send-communication" —
/// the queue the Azure Function actually dispatches from. This consumer is the only sanctioned
/// producer of "send-communication" messages.
/// </summary>
public sealed class RequestCommunicationConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RequestCommunicationConsumer> _logger;

    public RequestCommunicationConsumer(
        ServiceBusClient client,
        IOptions<RequestCommunicationQueueOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RequestCommunicationConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.QueueName))
        {
            throw new InvalidOperationException(
                "RequestCommunicationQueue:QueueName must be configured.");
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
        var input = args.Message.Body.ToObjectFromJson<RequestCommunicationInput>();

        using var scope = _scopeFactory.CreateScope();
        var requestCommunicationService = scope.ServiceProvider.GetRequiredService<IRequestCommunicationService>();

        var result = await requestCommunicationService.RequestAsync(input, args.CancellationToken);

        if (result.IsSuccess)
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        else
        {
            _logger.LogError("Failed to process communication request for rule {RuleId}: {Errors}.",
                input.RuleId, string.Join(", ", result.Errors.Select(e => e.Message)));

            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing request-communication message.");
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
