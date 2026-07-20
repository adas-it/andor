using Andor.Communications.Domain;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.ValueObjects;
using Andor.Communications.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Communications.Infrastructure;

public class CommandsRuleRepository(CommunicationContext context) : ICommandsRuleRepository
{
    protected readonly DbSet<Rule> DbSet = context.Set<Rule>();

    public Task<Rule?> GetByIdAsync(RuleId id, CancellationToken cancellationToken)
    {
        var entity = DbSet
            .Include(x => x.Templates)
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult<Rule?>(entity);
    }

    public async Task PersistAsync(Rule entity, CancellationToken cancellationToken)
    {
        context.Upsert<Rule, RuleId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
