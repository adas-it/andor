namespace Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface ICurrencyRepository : IRepository<Currency>, ISearchableRepository<Currency, SearchInput>
{
    Task<List<Currency>> GetByName(string name, CancellationToken cancellationToken);
}
