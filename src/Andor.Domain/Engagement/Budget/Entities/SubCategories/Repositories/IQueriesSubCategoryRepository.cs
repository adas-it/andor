using Andor.Domain.Engagement.Budget.Entities.SubCategories;
using Andor.Domain.Engagement.Budget.Entities.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface IQueriesSubCategoryRepository :
    IResearchableRepository<SubCategory, SubCategoryId, SearchInput>
{
    Task<List<SubCategory>> GetByIdsAsync(List<SubCategoryId> ids, CancellationToken cancellationToken);
}
