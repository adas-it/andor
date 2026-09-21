using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Onboarding.Application.Interfaces;

/// <summary>
/// Requests a communication be sent via Communications.Service's POST /v1/communications/requests
/// — the sanctioned way to land a message on its "request-communication" queue. Callers no longer
/// publish to that queue themselves; the endpoint enriches from its Recipient projection and gates
/// Marketing sends on consent.
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
