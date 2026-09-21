using Andor.Application.Communications.Interfaces;
using Andor.Communications.Domain;
using Andor.Communications.Domain.Users.ValueObjects;

namespace Andor.Application.Communications.Services.PartnerHandler;

public class InHousePartner(ISMTP _smtp) : IPartner
{
    public async Task SendAsync(RecipientId? recipientId,
        string? recipientEmail,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            throw new InvalidOperationException("InHousePartner requires a recipient email address.");
        }

        var body = TemplateRendering.Render(template.Value, values);
        var subject = TemplateRendering.Render(template.Subject, values);

        await _smtp.Handler(recipientEmail, body, subject, cancellationToken);
    }
}