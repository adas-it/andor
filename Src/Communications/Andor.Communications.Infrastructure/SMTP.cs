using System.Net.Mail;
using Andor.Application.Common;
using Andor.Application.Communications.Interfaces;
using Microsoft.Extensions.Options;

namespace Andor.Infrastructure.Communication.Gateway;

public class Smtp(IOptions<ApplicationSettings> configuration) : ISMTP
{
    public async Task Handler(string recipientMail,
        string body,
        string Subject,
        CancellationToken cancellationToken)
    {
        using SmtpClient client = new(configuration.Value.SmtpConfig!.Smtp!,
            configuration.Value.SmtpConfig.Port!.Value);

        MailAddress from = new(configuration.Value.SmtpConfig.Username!,
           configuration.Value.SmtpConfig.DisplayName, System.Text.Encoding.UTF8);

        if (!string.IsNullOrEmpty(configuration.Value.SmtpConfig.EmailTest))
        {
            recipientMail = configuration.Value.SmtpConfig.EmailTest;
        }

        MailAddress to = new(recipientMail);
        using MailMessage message = new(from, to);

        message.Body = body;
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.IsBodyHtml = true;
        message.Subject = Subject;
        message.SubjectEncoding = System.Text.Encoding.UTF8;

        client.EnableSsl = true;
        client.Credentials = new System.Net.NetworkCredential(configuration.Value.SmtpConfig.Username,
            configuration.Value.SmtpConfig.Password);

        await client.SendMailAsync(message, cancellationToken);
    }
}
