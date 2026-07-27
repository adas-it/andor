using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.SubCategories.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public class AccountSubCategoriesQueriesService(IAccountQueriesRepository accountQueriesRepository,
    ICurrentUserService currentUserService) : IAccountSubCategoriesQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ApplicationResult<ListSubCategoriesOutput?>> GetByAccountIdAndSubCategoryAsync(ListSubCategoriesQuery query, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(query.AccountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<ListSubCategoriesOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12222), $"Account with ID '{query.AccountId}' not found or user does not have access.") });
        }

        var querable = account.SubCategories.Select(x => x.SubCategory.ToSubCategoryOutput(x.Order))
            .Where(x => x.Name.Contains(query.Search ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        if (query.CategoryId.HasValue)
        {
            querable = querable.Where(x => x.Category.Id == query.CategoryId.Value.ToString());
        }

        if (query.Order == SearchOrder.Desc)
        {
            querable = querable.OrderByDescending(x => x.Name);
        }
        else
        {
            querable = querable.OrderBy(x => x.Name);
        }

        var accountSubCategories = querable.ToList();

        if (accountSubCategories is null || accountSubCategories.Count == 0)
        {
            return ApplicationResult<ListSubCategoriesOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12223), $"Sub-category with ID '{query.CategoryId}' not found for account with ID '{query.AccountId}'.") });
        }

        var output = new ListSubCategoriesOutput(query.Page, query.PerPage, accountSubCategories.Count,
            accountSubCategories.Skip(query.Page * query.PerPage).Take(query.PerPage).ToList());

        return ApplicationResult<ListSubCategoriesOutput?>.Success().SetData(output);
    }

    public async Task<ApplicationResult<SubCategoryOutput?>> GetByAccountIdAndSubCategoryIdAsync(AccountId accountId, SubCategoryId subCategoryId, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<SubCategoryOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12222), $"Account with ID '{accountId}' not found or user does not have access.") });
        }

        var accountSubCategory = account.SubCategories.FirstOrDefault(x => x.SubCategoryId == subCategoryId);

        if (accountSubCategory is null || accountSubCategory.SubCategory is null)
        {
            return ApplicationResult<SubCategoryOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12223), $"Sub-category with ID '{subCategoryId}' not found for account with ID '{accountId}'.") });
        }

        var subCategory = accountSubCategory.SubCategory.ToSubCategoryOutput(accountSubCategory.Order);

        return ApplicationResult<SubCategoryOutput?>.Success().SetData(subCategory);
    }
}
