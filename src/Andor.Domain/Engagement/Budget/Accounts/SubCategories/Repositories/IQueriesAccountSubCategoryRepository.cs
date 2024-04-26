using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;

public interface IQueriesAccountSubCategoryRepository
{
    Task<SubCategory?> GetByIdAsync(AccountId accountId, SubCategoryId subCategoryId, CancellationToken cancellationToken);

    Task<SearchOutput<SubCategory>> SearchAsync(SearchInputSubCategory input, CancellationToken cancellationToken);
}
