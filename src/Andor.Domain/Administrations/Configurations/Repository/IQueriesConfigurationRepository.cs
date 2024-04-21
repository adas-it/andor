using Andor.Domain.Administrations.Configurations.ValueObjects;
using Andor.Domain.Onboarding.Registrations.Repositories.Models;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Administrations.Configurations.Repository;

public interface IQueriesConfigurationRepository :
    IResearchableRepository<Configuration, ConfigurationId, SearchInput>
{
    Task<List<Configuration>?> GetByNameAndStatusAsync(SearchConfigurationInput search,
        CancellationToken cancellationToken);

    Task<Configuration?> GetActiveByNameAsync(string name,
        CancellationToken cancellationToken);
}
