using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;

public interface IQueriesAccountCategoryRepository
{
    Task<Category?> GetByIdAsync(AccountId accountId, CategoryId categoryId, CancellationToken cancellationToken);

    Task<SearchOutput<Category>> SearchAsync(SearchInputCategory input, CancellationToken cancellationToken);
}
