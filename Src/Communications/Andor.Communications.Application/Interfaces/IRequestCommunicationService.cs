using Andor.Communications.Contracts.Requests;
using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Application.Interfaces;

/// <summary>
/// Entry point for requesting a communication be sent: enriches from the Recipient projection,
/// gates Marketing sends on consent, and is the only thing allowed to publish onto the
/// "request-communication" queue — callers ask this instead of publishing themselves.
/// </summary>
public interface IRequestCommunicationService
{
    Task<ApplicationResult<object?>> RequestAsync(RequestCommunicationInput input, CancellationToken cancellationToken);
}
