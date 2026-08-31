using Andor.Authorizations.Domain;
using Andor.Communications.Application.Commands;
using Andor.Communications.Application.Interfaces;
using Andor.Communications.Contracts.Requests;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Infrastructure.Messaging;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Communications.Service.Consumers;

/// <summary>
/// Consumes the "request-communication" queue (populated by any module's Outbox) and sends
/// notifications using the Rules/Templates system. This provides a generic, decoupled way for
/// any bounded context to request communications without knowing the internal details.
/// </summary>
public sealed class RequestCommunicationConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RequestCommunicationConsumer> _logger;

    public RequestCommunicationConsumer(
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RequestCommunicationConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var queueName = options.Value.QueueName;

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException(
                "ServiceBus:QueueName must be configured for Communications to consume communication requests.");
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
        var message = args.Message.Body.ToObjectFromJson<SendNotificationInput>();

        using var scope = _scopeFactory.CreateScope();
        var ruleCommandsService = scope.ServiceProvider.GetRequiredService<IRuleCommandsService>();

        var systemUser = new ApplicationUser(Guid.Empty, "System", true, "TenantA");

        var command = new SendNotificationCommand(
            Id: RuleId.Load(message.RuleId),
            RecipientEmail: message.RecipientEmail,
            TemplateTitle: message.TemplateTitle,
            ContentLanguage: message.ContentLanguage,
            Values: message.Values,
            CurrentUser: systemUser,
            CancellationToken: args.CancellationToken);

        var result = await ruleCommandsService.SendNotificationAsync(command);

        if (result.IsSuccess)
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        else
        {
            _logger.LogError("Failed to send notification. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Message)));

            // Let the message be reprocessed or moved to dead-letter queue based on ServiceBus configuration
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing communication request message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
