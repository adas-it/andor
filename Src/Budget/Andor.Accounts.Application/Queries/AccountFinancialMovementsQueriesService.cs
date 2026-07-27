using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.FinancialMovements.Response;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.Repositories;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Queries;

public class AccountFinancialMovementsQueriesService(IAccountQueriesRepository accountQueriesRepository,
    IFinancialMovementQueriesRepository financialMovementQueriesRepository,
    ICurrentUserService currentUserService) : IAccountFinancialMovementsQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly IFinancialMovementQueriesRepository _financialMovementQueriesRepository = financialMovementQueriesRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ApplicationResult<ListFinancialMovementsOutput?>> GetByAccountIdAndFinancialMovementAsync(ListFinancialMovementsQuery query, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(query.AccountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<ListFinancialMovementsOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12228), $"Account with ID '{query.AccountId}' not found or user does not have access.") });
        }

        var movements = await _financialMovementQueriesRepository.GetAllFinancialMovementsByMonth(
            query.AccountId, Month.Load(query.Month), new Year(query.Year), cancellationToken);

        var querable = movements.Select(x => x.ToFinancialMovementOutput())
            .Where(x => (x!.Description ?? string.Empty).Contains(query.Search ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        querable = query.Order == SearchOrder.Desc
            ? querable.OrderByDescending(x => x!.Date)
            : querable.OrderBy(x => x!.Date);

        var items = querable.ToList();

        var output = new ListFinancialMovementsOutput(query.Page, query.PerPage, items.Count,
            items.Skip(query.Page * query.PerPage).Take(query.PerPage).ToList()!);

        return ApplicationResult<ListFinancialMovementsOutput?>.Success().SetData(output);
    }

    public async Task<ApplicationResult<FinancialMovementOutput?>> GetByAccountIdAndFinancialMovementIdAsync(AccountId accountId, FinancialMovementId financialMovementId, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<FinancialMovementOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12228), $"Account with ID '{accountId}' not found or user does not have access.") });
        }

        var movement = await _financialMovementQueriesRepository.GetByIdAsync(financialMovementId, cancellationToken);

        if (movement is null || movement.AccountId != accountId)
        {
            return ApplicationResult<FinancialMovementOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12229), $"Financial movement with ID '{financialMovementId}' not found for account with ID '{accountId}'.") });
        }

        return ApplicationResult<FinancialMovementOutput?>.Success().SetData(movement.ToFinancialMovementOutput());
    }
}
