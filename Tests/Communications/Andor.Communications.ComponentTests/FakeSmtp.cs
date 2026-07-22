using Andor.Application.Communications.Interfaces;

namespace Andor.Communications.ComponentTests;

/// <summary>
/// Replaces the real SMTP sender: production's <c>Smtp</c> opens a real, blocking
/// <see cref="System.Net.Mail.SmtpClient"/> connection using the credentials checked into
/// appsettings, which component tests must never touch.
/// </summary>
public sealed class FakeSmtp : ISMTP
{
    public List<(string RecipientEmail, string Body, string Subject)> SentMessages { get; } = [];

    public Task Handler(string recipientMail, string body, string subject, CancellationToken cancellationToken)
    {
        SentMessages.Add((recipientMail, body, subject));
        return Task.CompletedTask;
    }
}
