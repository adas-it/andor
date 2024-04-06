namespace Andor.Application.Communications.Interfaces;

public interface ISMTP
{
    Task Handler(string recipientMail, string body, string Subject, CancellationToken cancellationToken);
}
