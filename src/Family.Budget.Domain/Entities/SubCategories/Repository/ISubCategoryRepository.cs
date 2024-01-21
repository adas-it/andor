namespace Family.Budget.Domain.Entities.SubCategories.Repository;

using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface ISubCategoryRepository : IRepository<SubCategory>, ISearchableRepository<SubCategory, SearchInput>
{
    Task<List<SubCategory>> GetByName(string name, CancellationToken cancellationToken);
    Task<SearchOutput<SubCategory>> GetByCategory(SearchInput input, Guid categoryId, CancellationToken cancellationToken);
    Task<List<SubCategory>> GetByIds(List<Guid> Ids, CancellationToken cancellationToken);
}
