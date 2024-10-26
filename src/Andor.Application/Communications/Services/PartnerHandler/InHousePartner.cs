using Andor.Application.Communications.Interfaces;
using Andor.Domain.Communications;

namespace Andor.Application.Communications.Services.PartnerHandler;

public class InHousePartner(ISMTP _smtp) : IPartner
{
    public async Task SendEmail(string recipientEmail,
        string subject,
        Template template,
        Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        var body = template.Value;

        if (values is not null && values.Any())
        {
            values.ToList().ForEach(x => body = body.Replace(x.Key, x.Value));
        }

        await _smtp.Handler(recipientEmail, body, subject, cancellationToken);
    }
}