using Andor.Communications.Contracts.Requests;
using Andor.Foundation.Application;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Domain.Errors;

namespace Andor.Onboarding.Infrastructure;

public sealed class CommunicationRequestClient(IMessageSenderInterface messageSender)
    : ICommunicationRequestClient
{
    private const string QueueKey = "RequestCommunication";

    public async Task<DomainResult> RequestAsync(
        Guid ruleId,
        string templateTitle,
        Guid? userId,
        string? recipientEmail,
        string? contentLanguage,
        Dictionary<string, string>? values,
        CancellationToken cancellationToken)
    {
        var input = new RequestCommunicationInput(ruleId, templateTitle, userId, recipientEmail, contentLanguage, values);

        try
        {
            await messageSender.QueueSendAsync(QueueKey, input, Guid.NewGuid().ToString("N"), cancellationToken);
        }
        catch (Exception ex)
        {
            return DomainResult.Failure(errors: new[]
            {
                new Notification(
                    nameof(ICommunicationRequestClient),
                    $"Failed to publish onto the request-communication queue for rule '{ruleId}': {ex.Message}",
                    SignupErrorCodes.CommunicationRequestFailed),
            });
        }

        return DomainResult.Success();
    }
}
