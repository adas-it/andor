using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.Categories.Response;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public class AccountCategoriesQueriesService(IAccountQueriesRepository accountQueriesRepository,
    ICurrentUserService currentUserService) : IAccountCategoriesQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ApplicationResult<ListCategoriesOutput?>> GetByAccountIdAndCategoryAsync(ListCategoriesQuery query, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(query.AccountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<ListCategoriesOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12224), $"Account with ID '{query.AccountId}' not found or user does not have access.") });
        }

        var querable = account.Categories.Select(x => x.Category.ToCategoryOutput(x.Order))
            .Where(x => x.Name.Contains(query.Search ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        if (query.Type.HasValue)
        {
            querable = querable.Where(x => x.Type.Key == query.Type.Value);
        }

        querable = query.Order == SearchOrder.Desc
            ? querable.OrderByDescending(x => x.Name)
            : querable.OrderBy(x => x.Name);

        var categories = querable.ToList();

        if (categories.Count == 0)
        {
            return ApplicationResult<ListCategoriesOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12225), $"No categories found for account with ID '{query.AccountId}'.") });
        }

        var output = new ListCategoriesOutput(query.Page, query.PerPage, categories.Count,
            categories.Skip(query.Page * query.PerPage).Take(query.PerPage).ToList());

        return ApplicationResult<ListCategoriesOutput?>.Success().SetData(output);
    }

    public async Task<ApplicationResult<CategoryOutput?>> GetByAccountIdAndCategoryIdAsync(AccountId accountId, CategoryId categoryId, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<CategoryOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12224), $"Account with ID '{accountId}' not found or user does not have access.") });
        }

        var accountCategory = account.Categories.FirstOrDefault(x => x.CategoryId == categoryId);

        if (accountCategory is null || accountCategory.Category is null)
        {
            return ApplicationResult<CategoryOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12225), $"Category with ID '{categoryId}' not found for account with ID '{accountId}'.") });
        }

        var category = accountCategory.Category.ToCategoryOutput(accountCategory.Order);

        return ApplicationResult<CategoryOutput?>.Success().SetData(category);
    }
}
