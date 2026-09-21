using Andor.Communications.Domain;
using Andor.Communications.Domain.Users.ValueObjects;

namespace Andor.Application.Communications.Interfaces;

public interface IPartner
{
    /// <summary>
    /// <paramref name="recipientId"/> and <paramref name="recipientEmail"/> are both nullable
    /// since which one a partner actually needs depends on its channel — InHousePartner (email)
    /// needs an address; PushPartner needs a RecipientId to record the message against.
    /// </summary>
    Task SendAsync(RecipientId? recipientId,
        string? recipientEmail,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken);
}
