using Andor.Communications.Domain.Messages;
using Andor.Communications.Domain.Users.ValueObjects;

namespace Andor.Communications.Domain.Repositories;

public interface ICommandsMessageRepository
{
    Task PersistAsync(Message entity, CancellationToken cancellationToken);

    Task<IReadOnlyList<Message>> GetByRecipientIdAsync(RecipientId recipientId, CancellationToken cancellationToken);
}
