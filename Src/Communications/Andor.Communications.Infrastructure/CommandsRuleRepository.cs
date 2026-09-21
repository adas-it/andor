using Andor.Communications.Domain;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.ValueObjects;
using Andor.Communications.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Andor.Communications.Infrastructure;

public class CommandsRuleRepository(CommunicationContext context, IMemoryCache cache) : ICommandsRuleRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    protected readonly DbSet<Rule> DbSet = context.Set<Rule>();

    public async Task<Rule?> GetByIdAsync(RuleId id, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKey(id);

        if (cache.TryGetValue(cacheKey, out Rule? cached))
        {
            return cached;
        }

        var entity = await DbSet
            .Include(x => x.Templates)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        cache.Set(cacheKey, entity, CacheDuration);

        return entity;
    }

    public async Task PersistAsync(Rule entity, CancellationToken cancellationToken)
    {
        context.Upsert<Rule, RuleId>(entity);

        await context.SaveChangesAsync(cancellationToken);

        cache.Remove(CacheKey(entity.Id));
    }

    private static string CacheKey(RuleId id) => $"Rule:{id.Value}";
}
