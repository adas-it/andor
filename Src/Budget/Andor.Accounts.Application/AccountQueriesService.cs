using System.Globalization;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Accounts.Domain.FinancialMovements.Repositories;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application;

public class AccountQueriesService(IAccountQueriesRepository accountQueriesRepository,
    ICurrentUserService currentUser,
    ICommandsCashFlowRepository commandsCashFlowRepository,
    IFinancialMovementQueriesRepository financialMovementQueriesRepository) : IAccountQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ICommandsCashFlowRepository _commandsCashFlowRepository = commandsCashFlowRepository;
    private readonly IFinancialMovementQueriesRepository _financialMovementQueriesRepository = financialMovementQueriesRepository;

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

    public async Task<ApplicationResult<List<FinancialSummariesOutput>>> GetFinancialSummaryAsync(AccountId accountId, Month month, Year year, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (result is null || !result.Members.Any(x => x.UserId == userId))
        {
            return ApplicationResult<List<FinancialSummariesOutput>>.Failure();
        }

        var response = ApplicationResult<List<FinancialSummariesOutput>>.Success();

        var listFinancialMovements = await _financialMovementQueriesRepository.GetAllFinancialMovementsByMonth(
            accountId,
            month,
            year,
            cancellationToken);

        var it = listFinancialMovements.Where(x => x.Status.Key == MovementStatus.Accomplished.Key).Select(x => new
        {
            Category = new FinancialSummariesOutput.SummariesOutput(x.SubCategory.Category.Id.ToString(), x.SubCategory.Category.Name, 0),
            SubCategory = new FinancialSummariesOutput.SummariesOutput(x.SubCategory.Id.ToString(), x.SubCategory.Name, 0),
            CategoryType = new KeyValuePair<int, string>(x.SubCategory.Category.Type.Key, x.SubCategory.Category.Type.Name),
            Week = GetWeekOfMonth(x.Date),
            Value = x.Value
        }).ToList();

        var items = it.GroupBy(x => new { x.Category, x.SubCategory, x.CategoryType })
            .Select(x => new FinancialSummariesOutput()
            {
                Category = x.Key.Category,
                SubCategory = x.Key.SubCategory,
                CategoryType = new KeyValuePair<int, string>(x.Key.CategoryType.Key, x.Key.CategoryType.Value),
                Week1 = x.Where(z => z.Week == 1).Sum(z => z.Value),
                Week2 = x.Where(z => z.Week == 2).Sum(z => z.Value),
                Week3 = x.Where(z => z.Week == 3).Sum(z => z.Value),
                Week4 = x.Where(z => z.Week == 4).Sum(z => z.Value),
                Week5 = x.Where(z => z.Week == 5).Sum(z => z.Value)
            });

        var output = items
            .OrderBy(x => x.CategoryType.Key).ToList();

        response.SetData(output);

        return response;
    }

    public async Task<ApplicationResult<List<CategorySummariesOutput>>> GetCategorySummaryAsync(AccountId accountId, Month month, Year year, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetCurrentUser().UserId;

        var result = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (result is null || !result.Members.Any(x => x.UserId == userId))
        {
            return ApplicationResult<List<CategorySummariesOutput>>.Failure();
        }

        var listFinancialMovements = await _financialMovementQueriesRepository.GetAllFinancialMovementsByMonth(
            accountId,
            month,
            year,
            cancellationToken);

        var items = listFinancialMovements.Where(x => x.Status.Key == MovementStatus.Accomplished.Key).Select(x => new
        {
            Category = new CategorySummariesOutput.CategoryOutput(x.SubCategory.Category.Id.ToString(), x.SubCategory.Category.Name, 0),
            CategoryType = new KeyValuePair<int, string>(x.SubCategory.Category.Type.Key, x.SubCategory.Category.Type.Name),
            Value = x.Value
        }).GroupBy(x => new { x.Category, x.CategoryType })
            .Select(x => new CategorySummariesOutput()
            {
                Category = x.Key.Category,
                CategoryType = new KeyValuePair<int, string>(x.Key.CategoryType.Key, x.Key.CategoryType.Value),
                Value = x.Sum(z => z.Value)
            }).ToList();

        var output = items
            .OrderBy(x => x.CategoryType.Key)
            .ThenBy(x => x.Category.Order).ToList();

        return output;
    }
    private static int GetWeekOfMonth(DateTime date)
    {
        DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        int firstDayOfWeek = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        int offset = (int)firstDayOfMonth.DayOfWeek - firstDayOfWeek;
        offset = offset < 0 ? offset + 7 : offset;

        int weekOfMonth = (date.Day + offset - 1) / 7 + 1;

        if (weekOfMonth > 5)
        {
            weekOfMonth = 5;
        }

        return weekOfMonth;
    }
}
