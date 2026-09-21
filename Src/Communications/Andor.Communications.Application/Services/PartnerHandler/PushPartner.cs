using Andor.Application.Communications.Interfaces;
using Andor.Communications.Domain;
using Andor.Communications.Domain.Messages;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users.ValueObjects;

namespace Andor.Application.Communications.Services.PartnerHandler;

/// <summary>
/// Records a push notification for in-app delivery instead of calling an external push provider
/// (FCM/APNs aren't wired up yet) — the recipient's app lists these via
/// GET /v1/communications/messages.
/// </summary>
public class PushPartner(ICommandsMessageRepository messageRepository) : IPartner
{
    public async Task SendAsync(RecipientId? recipientId,
        string? recipientEmail,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        if (recipientId is not { } id)
        {
            throw new InvalidOperationException("PushPartner requires a RecipientId.");
        }

        var title = TemplateRendering.Render(template.Subject, values);
        var body = TemplateRendering.Render(template.Value, values);

        var (result, message) = Message.New(id, title, body);

        if (result.IsSuccess && message is not null)
        {
            await messageRepository.PersistAsync(message, cancellationToken);
        }
    }
}
