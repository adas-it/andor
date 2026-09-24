using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Onboarding.Application.Interfaces;

/// <summary>
/// Requests a communication be sent by publishing directly onto Communications' "request-communication"
/// queue. Communications.Service consumes that queue, enriches from its Recipient projection, gates
/// Marketing sends on consent, and republishes onto "send-communication" — the only queue its Azure
/// Function actually dispatches from.
/// </summary>
public interface ICommunicationRequestClient
{
    Task<DomainResult> RequestAsync(
        Guid ruleId,
        string templateTitle,
        Guid? userId,
        string? recipientEmail,
        string? contentLanguage,
        Dictionary<string, string>? values,
        CancellationToken cancellationToken);
}
