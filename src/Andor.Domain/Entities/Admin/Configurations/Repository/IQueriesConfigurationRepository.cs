using Andor.Domain.Entities.Admin.Configurations.ValueObjects;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories.Models;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Entities.Admin.Configurations.Repository;

public interface IQueriesConfigurationRepository :
    IResearchableRepository<Configuration, ConfigurationId, SearchInput>
{
    Task<List<Configuration>?> GetByNameAndStatusAsync(SearchConfigurationInput search,
        CancellationToken cancellationToken);

    Task<Configuration?> GetActiveByNameAsync(string name,
        CancellationToken cancellationToken);
}
