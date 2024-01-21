namespace Family.Budget.Domain.Entities.Admin.Repository;

using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface IConfigurationRepository : IRepository<Configuration>, ISearchableRepository<Configuration, SearchInput>
{
    Task<List<Configuration>> GetByName(string name, CancellationToken cancellationToken);
    Task<Configuration?> GetByNameActive(string name, CancellationToken cancellationToken);
}