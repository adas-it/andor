using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.SubCategories.Repositories;

public interface IQueriesSubCategoryRepository :
    IResearchableRepository<SubCategory, SubCategoryId, SearchInput>
{
    Task<List<SubCategory>> GetByIdsAsync(List<SubCategoryId> ids, CancellationToken cancellationToken);
}
