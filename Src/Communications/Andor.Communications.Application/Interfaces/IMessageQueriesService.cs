using Andor.Communications.Contracts.Responses;
using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Application.Interfaces;

public interface IMessageQueriesService
{
    Task<ApplicationResult<IReadOnlyList<MessageOutput>?>> GetByRecipientAsync(Guid recipientId, CancellationToken cancellationToken);
}
