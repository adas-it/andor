namespace Family.Budget.Domain.Entities.Categories.Repository;

using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface ICategoryRepository : IRepository<Category>, ISearchableRepository<Category, SearchInputCategory>
{
    Task<List<Category>> GetByName(string name, CancellationToken cancellationToken);
}
