using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application;

public class AccountQueriesService(IAccountQueriesRepository accountQueriesRepository,
    ICurrentUserService currentUser,
    ICommandsCashFlowRepository commandsCashFlowRepository) : IAccountQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ICommandsCashFlowRepository _commandsCashFlowRepository = commandsCashFlowRepository;

    public async Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken)
    {
        var UserId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.GetByIdAsync(id, cancellationToken);

        var resultAccount = result?.ToAccountOutput();

        if (result is not null && result.Members.Any(x => x.UserId == UserId))
        {

            return ApplicationResult<AccountOutput>.Success(Data: resultAccount);
        }

        return ApplicationResult<AccountOutput>.Failure();
    }

    public async Task<ApplicationResult<ListAccountOutput>> GetListAsync(SearchInput input, CancellationToken cancellationToken)
    {
        var UserId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.SearchAsync(input, cancellationToken);

        var resultAccount = new ListAccountOutput(result.CurrentPage, result.PerPage, result.Total,
            result.Items.Where(x => x.Members.Any(m => m.UserId == UserId)).Select(x => x.ToAccountOutput()).ToList());

        return ApplicationResult<ListAccountOutput>.Success(Data: resultAccount);
    }

    public async Task<ApplicationResult<CashFlowOutput>> GetCashFlowAsync(AccountId accountId, Month month, Year year, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (result is null || !result.Members.Any(x => x.UserId == userId))
        {
            return ApplicationResult<CashFlowOutput>.Failure();
        }

        var period = (year * 100) + month;

        var current = await _commandsCashFlowRepository.GetByAccountAndPeriodAsync(accountId, period, cancellationToken);

        if (current is not null)
        {
            return ApplicationResult<CashFlowOutput>.Success(Data: new CashFlowOutput()
            {
                Id = current.Id,
                Month = current.Month,
                Year = current.Year,
                MonthRevenues = current.MonthRevenues,
                FinalBalancePreviousMonth = current.FinalBalancePreviousMonth,
                ForecastUpcomingRevenues = current.ForecastUpcomingRevenues,
                RevenuesBalance = current.RevenuesBalance,
                Expenses = current.Expenses,
                AccountBalance = current.AccountBalance,
                ForecastExpenses = current.ForecastExpenses,
                BalanceForecast = current.BalanceForecast,
                MonthlyDeficitSurplus = current.MonthlyDeficitSurplus
            });

        }

        var lastOne = await _commandsCashFlowRepository.GetLatestBeforeAsync(accountId, period, cancellationToken);

        if (lastOne is not null)
        {
            return await Task.FromResult(ApplicationResult<CashFlowOutput>.Success(Data: new CashFlowOutput()
            {
                Id = Guid.NewGuid(),
                Month = month,
                Year = year,
                MonthRevenues = 0,
                FinalBalancePreviousMonth = lastOne.AccountBalance,
                ForecastUpcomingRevenues = 0,
                RevenuesBalance = 0,
                Expenses = 0,
                AccountBalance = 0,
                ForecastExpenses = 0,
                BalanceForecast = 0,
                MonthlyDeficitSurplus = 0
            }));
        }

        return await Task.FromResult(ApplicationResult<CashFlowOutput>.Success(Data: new CashFlowOutput()
        {
            Id = Guid.NewGuid(),
            Month = month,
            Year = year,
            MonthRevenues = 0,
            FinalBalancePreviousMonth = 0,
            ForecastUpcomingRevenues = 0,
            RevenuesBalance = 0,
            Expenses = 0,
            AccountBalance = 0,
            ForecastExpenses = 0,
            BalanceForecast = 0,
            MonthlyDeficitSurplus = 0
        }));

    }

    public async Task<ApplicationResult<FinancialSummariesOutput>> GetFinancialSummaryAsync(AccountId accountId, Month month, Year year, CancellationToken cancellationToken)
    {
        return await Task.FromResult(ApplicationResult<FinancialSummariesOutput>.Success(Data: new FinancialSummariesOutput()));
    }

    public async Task<ApplicationResult<CategorySummariesOutput>> GetCategorySummaryAsync(AccountId accountId, Month month, Year year, CancellationToken cancellationToken)
    {
        return await Task.FromResult(ApplicationResult<CategorySummariesOutput>.Success(Data: new CategorySummariesOutput()));
    }
}
