using Andor.Communications.Domain.Messages;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Communications.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Communications.Infrastructure;

public class CommandsMessageRepository(CommunicationContext context) : ICommandsMessageRepository
{
    protected readonly DbSet<Message> DbSet = context.Set<Message>();

    public async Task PersistAsync(Message entity, CancellationToken cancellationToken)
    {
        _ = DbSet.Add(entity);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> GetByRecipientIdAsync(RecipientId recipientId, CancellationToken cancellationToken)
        => await DbSet
            .Where(x => x.RecipientId == recipientId)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync(cancellationToken);
}
