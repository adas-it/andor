using Andor.Application.Communications.Services.Manager;
using Andor.Communications.Contracts.Requests;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Communications.Domain.ValueObjects;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Andor.Communications.External;

/// <summary>
/// Reads the Rule/Template requested by a "request-communication" message and dispatches it
/// through the Partner strategy directly, bypassing the Rules actor system: SendNotification
/// never mutates the Rule aggregate, so it doesn't need the Manager/Actor/Stash write path.
/// </summary>
public class RequestCommunicationFunction(
    ICommandsRuleRepository ruleRepository,
    IPartnerManager partnerManager,
    IMemoryCache cache,
    ILogger<RequestCommunicationFunction> logger)
{
    [Function(nameof(RequestCommunicationFunction))]
    public async Task Run(
        [ServiceBusTrigger("request-communication", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        var dedupeKey = $"request-communication:processed:{message.MessageId}";

        if (cache.TryGetValue(dedupeKey, out _))
        {
            logger.LogInformation("Message {MessageId} already processed, skipping.", message.MessageId);
            await messageActions.CompleteMessageAsync(message, cancellationToken);
            return;
        }

        var input = message.Body.ToObjectFromJson<SendNotificationInput>();

        try
        {
            var rule = await ruleRepository.GetByIdAsync(RuleId.Load(input.RuleId), cancellationToken);

            var template = rule?.Templates.FirstOrDefault(x =>
                x.Title == input.TemplateTitle && x.ContentLanguage == input.ContentLanguage);

            if (template is null)
            {
                logger.LogError(
                    "Template '{TemplateTitle}'/'{ContentLanguage}' not found for rule '{RuleId}'.",
                    input.TemplateTitle, input.ContentLanguage, input.RuleId);
                await messageActions.AbandonMessageAsync(message, cancellationToken: cancellationToken);
                return;
            }

            var partner = partnerManager.GetPartnerHandler(template.Partner);

            var recipientId = input.RecipientId is { } id ? RecipientId.Load(id) : (RecipientId?)null;

            await partner.SendAsync(recipientId, input.RecipientEmail, template, input.Values, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process communication request {MessageId}.", message.MessageId);
            await messageActions.AbandonMessageAsync(message, cancellationToken: cancellationToken);
            return;
        }

        cache.Set(dedupeKey, true, TimeSpan.FromHours(1));

        await messageActions.CompleteMessageAsync(message, cancellationToken);
    }
}
