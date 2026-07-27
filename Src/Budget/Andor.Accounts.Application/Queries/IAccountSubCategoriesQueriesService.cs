using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.SubCategories.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public interface IAccountSubCategoriesQueriesService
{
    Task<ApplicationResult<ListSubCategoriesOutput?>> GetByAccountIdAndSubCategoryAsync(ListSubCategoriesQuery query, CancellationToken cancellationToken);
    Task<ApplicationResult<SubCategoryOutput?>> GetByAccountIdAndSubCategoryIdAsync(AccountId accountId, SubCategoryId subCategoryId, CancellationToken cancellationToken);
}