namespace Family.Budget.Application.FinancialSummaries.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Dto.Common.Response;
using Family.Budget.Application.Dto.FinancialSummaries.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

public record GetFinancialSummariesByMonthQuery : IRequest<List<FinancialSummariesOutput>>
{
    public Guid AccountId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetFinancialSummariesByMonthHandler : BaseCommands, IRequestHandler<GetFinancialSummariesByMonthQuery, List<FinancialSummariesOutput>>
{
    private readonly IFinancialMovementRepository _repository;

    public GetFinancialSummariesByMonthHandler(IFinancialMovementRepository repository, Notifier notifier) : base(notifier)
    {
        _repository = repository;
    }

    public async Task<List<FinancialSummariesOutput>> Handle(GetFinancialSummariesByMonthQuery request, CancellationToken cancellationToken)
    {
        var listFinancialMovements = await _repository.GetAllFinancialMovementsByMonth(
            request.AccountId,
            request.Year,
            request.Month,
            cancellationToken);

        var it = listFinancialMovements.Where(x => x.Status == MovementStatus.Accomplished).Select(x => new
        {
            Category = new KeyValuePairModelGuidString(x.SubCategory.Category.Id, x.SubCategory.Category.Name),
            SubCategory = new KeyValuePairModelGuidString(x.SubCategory.Id, x.SubCategory.Name),
            CategoryType = new KeyValuePairModelIntString(x.SubCategory.Category.Type.Key, x.SubCategory.Category.Type.Name),
            Week = GetWeekOfMonth(x.Date),
            Value = x.Value
        }).ToList();

        var items = it.GroupBy(x => new { x.Category, x.SubCategory, x.CategoryType })
        .Select(x => new FinancialSummariesOutput()
        {
            Category = new KeyValuePairModel<Guid, string>() { Key = x.Key.Category.Key, Value = x.Key.Category.Value },
            SubCategory = new KeyValuePairModel<Guid, string>() { Key = x.Key.SubCategory.Key, Value = x.Key.SubCategory.Value },
            CategoryType = new KeyValuePairModel<int, string>() { Key = x.Key.CategoryType.Key, Value = x.Key.CategoryType.Value },
            Week1 = x.Where(z => z.Week == 1).Sum(z => z.Value),
            Week2 = x.Where(z => z.Week == 2).Sum(z => z.Value),
            Week3 = x.Where(z => z.Week == 3).Sum(z => z.Value),
            Week4 = x.Where(z => z.Week == 4).Sum(z => z.Value),
            Week5 = x.Where(z => z.Week == 5).Sum(z => z.Value),
            Week6 = x.Where(z => z.Week == 6).Sum(z => z.Value)
        });

        return items.OrderBy(x => x.Category.Key).ToList();
    }

    private static int GetWeekOfMonth(DateTimeOffset date)
    {
        int firstDayOfWeek = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        int weekOfMonth = (date.Day + (int)date.DayOfWeek - firstDayOfWeek) / 7 + 1;

        return weekOfMonth;
    }

    private record KeyValuePairModelGuidString(Guid Key, string Value);
    private record KeyValuePairModelIntString(int Key, string Value);
}
