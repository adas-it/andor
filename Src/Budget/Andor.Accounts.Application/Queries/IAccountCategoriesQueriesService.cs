using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.Categories.Response;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public interface IAccountCategoriesQueriesService
{
    Task<ApplicationResult<ListCategoriesOutput?>> GetByAccountIdAndCategoryAsync(ListCategoriesQuery query, CancellationToken cancellationToken);
    Task<ApplicationResult<CategoryOutput?>> GetByAccountIdAndCategoryIdAsync(AccountId accountId, CategoryId categoryId, CancellationToken cancellationToken);
}
