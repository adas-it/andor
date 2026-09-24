using Andor.Communications.Contracts.Requests;
using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Application.Interfaces;

/// <summary>
/// Handles a message received on the "request-communication" queue: enriches from the Recipient
/// projection, gates Marketing sends on consent, and is the only thing allowed to publish onto the
/// "send-communication" queue that the Azure Function dispatches from.
/// </summary>
public interface IRequestCommunicationService
{
    Task<ApplicationResult<object?>> RequestAsync(RequestCommunicationInput input, CancellationToken cancellationToken);
}
