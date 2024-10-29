using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;

public interface IQueriesAccountSubCategoryRepository
{
    Task<SubCategory?> GetByIdAsync(AccountId accountId, SubCategoryId subCategoryId, CancellationToken cancellationToken);

    Task<ListSubCategoriesOutput> SearchAsync(SearchInputSubCategory input, CancellationToken cancellationToken);
}
