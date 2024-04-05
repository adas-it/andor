using Andor.Application.Common;
using Andor.Application.Communications.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net.Mail;

namespace Andor.Infrastructure.Communication.Gateway;

public class SMTP(IOptions<ApplicationSettings> configuration) : ISMTP
{
    public Task Handler(string recipientMail,
        string body,
        string Subject,
        CancellationToken cancellationToken)
    {
        // Command-line argument must be the SMTP host.
        SmtpClient client = new SmtpClient(configuration.Value.SmtpConfig.Smtp,
            configuration.Value.SmtpConfig.Port.Value);
        // Specify the email sender.
        // Create a mailing address that includes a UTF8 character
        // in the display name.
        MailAddress from = new MailAddress(configuration.Value.SmtpConfig.Username,
           configuration.Value.SmtpConfig.DisplayName,
        System.Text.Encoding.UTF8);
        // Set destinations for the email message.
        MailAddress to = new MailAddress(recipientMail);
        // Specify the message content.
        MailMessage message = new MailMessage(from, to);
        message.Body = body;
        // Include some non-ASCII characters in body and subject.
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.IsBodyHtml = true;

        message.Subject = Subject;
        message.SubjectEncoding = System.Text.Encoding.UTF8;
        // Set the method that is called back when the send operation ends.
        client.SendCompleted += new
        SendCompletedEventHandler(SendCompletedCallback);
        // The userState can be any object that allows your callback
        // method to identify this send operation.
        // For this example, the userToken is a string constant.

        client.EnableSsl = true;
        client.Credentials = new System.Net.NetworkCredential(configuration.Value.SmtpConfig.Username,
            configuration.Value.SmtpConfig.Password);

        client.Send(message);

        // Clean up.
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
