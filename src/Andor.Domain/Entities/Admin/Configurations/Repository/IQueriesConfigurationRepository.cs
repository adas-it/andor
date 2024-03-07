using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Entities.Admin.Configurations.Repository;

public interface IQueriesConfigurationRepository : 
    IResearchableRepository<Configuration, ConfigurationId, SearchInput>
{
    Task<List<Configuration>> GetAllByNameAsync(string name,
        ConfigurationState[] statuses,
        CancellationToken cancellationToken);
    Task<Configuration?> GetByNameAsync(string name,
        ConfigurationState[] statuses,
        CancellationToken cancellationToken);
}
