using Andor.Domain.Entities.Communications;

namespace Andor.Application.Communications.Interfaces;

public interface IPartner
{
    Task SendEmail(string recipientEmail,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken);
}
