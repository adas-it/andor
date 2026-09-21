using Andor.Communications.Application.Interfaces;
using Andor.Communications.Contracts.Responses;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Application;

public class MessageQueriesService(ICommandsMessageRepository messageRepository) : IMessageQueriesService
{
    public async Task<ApplicationResult<IReadOnlyList<MessageOutput>?>> GetByRecipientAsync(Guid recipientId,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<IReadOnlyList<MessageOutput>?>.Success();

        var messages = await messageRepository.GetByRecipientIdAsync(RecipientId.Load(recipientId), cancellationToken);

        var output = messages
            .Select(m => new MessageOutput(m.Id.Value, m.Title, m.Body, m.SentAt))
            .ToList();

        return response.SetData(output);
    }
}
