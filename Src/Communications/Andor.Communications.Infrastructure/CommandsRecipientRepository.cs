using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.Users;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Communications.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Communications.Infrastructure;

// Deliberately no caching here (unlike CommandsRuleRepository) - consent/active-status is exactly
// what this repository gates sends on, so a stale read is the one thing to avoid.
public class CommandsRecipientRepository(CommunicationContext context) : ICommandsRecipientRepository
{
    protected readonly DbSet<Recipient> DbSet = context.Set<Recipient>();

    public async Task<Recipient?> GetByIdAsync(RecipientId id, CancellationToken cancellationToken)
        => await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task PersistAsync(Recipient entity, CancellationToken cancellationToken)
    {
        context.Upsert<Recipient, RecipientId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
