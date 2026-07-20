using Andor.Communications.Domain;

namespace Andor.Application.Communications.Interfaces;

public interface IPartner
{
    Task SendEmail(string recipientEmail,
        string subject,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken);
}
