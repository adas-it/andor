using Andor.Domain.Communications;

namespace Andor.Application.Communications.Interfaces;

public interface IPartner
{
    Task SendEmail(string recipientEmail,
        string subject,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken);
}
