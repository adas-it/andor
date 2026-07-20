using Andor.Application.Common;
using Andor.Application.Communications.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net.Mail;

namespace Andor.Infrastructure.Communication.Gateway;

public class Smtp(IOptions<ApplicationSettings> configuration) : ISMTP
{
    public Task Handler(string recipientMail,
        string body,
        string Subject,
        CancellationToken cancellationToken)
    {
        SmtpClient client = new(configuration.Value.SmtpConfig!.Smtp!,
            configuration.Value.SmtpConfig.Port!.Value);

        MailAddress from = new(configuration.Value.SmtpConfig.Username!,
           configuration.Value.SmtpConfig.DisplayName, System.Text.Encoding.UTF8);

        MailAddress to = new(recipientMail);
        MailMessage message = new(from, to);

        message.Body = body;
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.IsBodyHtml = true;
        message.Subject = Subject;
        message.SubjectEncoding = System.Text.Encoding.UTF8;

        client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
        client.EnableSsl = true;
        client.Credentials = new System.Net.NetworkCredential(configuration.Value.SmtpConfig.Username,
            configuration.Value.SmtpConfig.Password);

        client.Send(message);

        message.Dispose();

        return Task.CompletedTask;
    }

    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        // Get the unique identifier for this asynchronous operation.
        string token = (string)e.UserState;

        if (e.Cancelled)
        {
            Console.WriteLine("[{0}] Send canceled.", token);
        }
        if (e.Error != null)
        {
            Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
        }
        else
        {
            Console.WriteLine("Message sent.");
        }
    }
}
