using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;

public interface IQueriesAccountCategoryRepository
{
    Task<Category?> GetByIdAsync(AccountId accountId, CategoryId categoryId, CancellationToken cancellationToken);

    Task<ListCategoriesOutput> SearchAsync(SearchInputCategory input, CancellationToken cancellationToken);
}
