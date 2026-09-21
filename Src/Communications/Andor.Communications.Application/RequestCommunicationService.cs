using Andor.Communications.Application.Interfaces;
using Andor.Communications.Contracts.Requests;
using Andor.Communications.Contracts.Responses;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using CommunicationType = Andor.Communications.Domain.ValueObjects.Type;

namespace Andor.Communications.Application;

public class RequestCommunicationService(
    ICommandsRuleRepository ruleRepository,
    ICommandsRecipientRepository recipientRepository,
    IMessageSenderInterface messageSender) : IRequestCommunicationService
{
    public async Task<ApplicationResult<object?>> RequestAsync(RequestCommunicationInput input,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<object?>.Success();

        var recipientEmail = input.RecipientEmail;
        var contentLanguage = input.ContentLanguage;
        Guid? recipientId = null;
        var values = input.Values is null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(input.Values);

        if (input.UserId is { } userId)
        {
            var recipient = await recipientRepository.GetByIdAsync(RecipientId.Load(userId), cancellationToken);

            if (recipient is null)
            {
                return response.AddError(RequestCommunicationErrors.RecipientNotFound());
            }

            if (!recipient.Active)
            {
                return response.AddError(RequestCommunicationErrors.RecipientInactive());
            }

            var rule = await ruleRepository.GetByIdAsync(RuleId.Load(input.RuleId), cancellationToken);

            if (rule is null)
            {
                return response.AddError(RuleErrors.RuleNotFound());
            }

            if (rule.Type == CommunicationType.Marketing && !recipient.MarketingOptIn)
            {
                return response.AddError(RequestCommunicationErrors.MarketingConsentRequired());
            }

            recipientEmail = recipient.Email;
            contentLanguage ??= recipient.PreferredLanguage;
            recipientId = recipient.Id.Value;
            values.TryAdd("<name>", recipient.Name);
        }

        if (string.IsNullOrWhiteSpace(recipientEmail) || string.IsNullOrWhiteSpace(contentLanguage))
        {
            return response.AddError(RequestCommunicationErrors.MissingRecipient());
        }

        var notification = new SendNotificationInput(
            input.RuleId, recipientEmail, input.TemplateTitle, contentLanguage, values, recipientId);

        await messageSender.QueueSendAsync(notification, Guid.NewGuid().ToString("N"), cancellationToken);

        return response;
    }
}
