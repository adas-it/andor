using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using MediatR;
using System.Globalization;

namespace Andor.Application.Engagement.Budget.MonthlyCash.Queries;

public record GetFinancialSummariesByMonthQuery : IRequest<ApplicationResult<List<FinancialSummariesOutput>>>
{
    public AccountId AccountId { get; set; }
    public Year Year { get; set; }
    public Month Month { get; set; }
}

public class GetFinancialSummariesByMonthHandler(IQueriesFinancialMovementRepository _repository)
    : IRequestHandler<GetFinancialSummariesByMonthQuery, ApplicationResult<List<FinancialSummariesOutput>>>
{
    public async Task<ApplicationResult<List<FinancialSummariesOutput>>> Handle(GetFinancialSummariesByMonthQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<List<FinancialSummariesOutput>>.Success();

        var listFinancialMovements = await _repository.GetAllFinancialMovementsByMonth(
            request.AccountId,
            request.Year,
            request.Month,
            cancellationToken);

        var it = listFinancialMovements.Where(x => x.Status.Key == MovementStatus.Accomplished.Key).Select(x => new
        {
            Category = new FinancialSummariesOutput.CategorySummarieOutuput(x.SubCategory.Category.Id, x.SubCategory.Category.Name, x.SubCategory.Order ?? 0),
            SubCategory = new FinancialSummariesOutput.CategorySummarieOutuput(x.SubCategory.Id, x.SubCategory.Name, x.SubCategory.Order ?? 0),
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
            .OrderBy(x => x.CategoryType.Key)
            .ThenBy(x => x.Category.order)
            .ThenBy(x => x.SubCategory.order).ToList();

        response.SetData(output);

        return response;
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
