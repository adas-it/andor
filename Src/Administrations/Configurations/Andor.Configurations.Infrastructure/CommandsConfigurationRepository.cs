using Andor.Configurations.Domain;
using Andor.Configurations.Domain.Repositories;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Configurations.Infrastructure.Context;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.EntityFrameworkCore;

namespace Andor.Configurations.Infrastructure;

public class CommandsConfigurationRepository(ConfigurationContext context) : ICommandsConfigurationRepository
{
    protected readonly DbSet<Configuration> DbSet = context.Set<Configuration>();

    public Task<Configuration?> GetByIdAsync(ConfigurationId id, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);

        return Task.FromResult<Configuration?>(entity);
    }

    public async Task PersistAsync(Configuration entity, CancellationToken cancellationToken)
    {
        context.Upsert<Configuration, ConfigurationId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Configuration>> GetByNameAndStatesAsync(Name name, ConfigurationState[] states,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<Configuration>());
    }
}
