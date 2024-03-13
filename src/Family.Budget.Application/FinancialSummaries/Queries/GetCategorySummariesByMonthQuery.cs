namespace Family.Budget.Application.FinancialSummaries.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Dto.Common.Response;
using Family.Budget.Application.Dto.FinancialSummaries.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public record GetCategorySummariesByMonthQuery : IRequest<List<CategorySummariesOutput>>
{
    public Guid AccountId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetFinancialSummariesByMonthQueryHandler : BaseCommands, IRequestHandler<GetCategorySummariesByMonthQuery, List<CategorySummariesOutput>>
{
    private readonly IFinancialMovementRepository _repository;

    public GetFinancialSummariesByMonthQueryHandler(IFinancialMovementRepository repository, Notifier notifier) : base(notifier)
    {
        _repository = repository;
    }

    public async Task<List<CategorySummariesOutput>> Handle(GetCategorySummariesByMonthQuery request, CancellationToken cancellationToken)
    {
        var listFinancialMovements = await _repository.GetAllFinancialMovementsByMonth(
            request.AccountId,
            request.Year,
            request.Month,
            cancellationToken);

        var items = listFinancialMovements.Select(x => new
        {
            Category = new KeyValuePairModelGuidString(x.SubCategory.Category.Id, x.SubCategory.Category.Name),
            CategoryType = new KeyValuePairModelIntString(x.SubCategory.Category.Type.Key, x.SubCategory.Category.Type.Name),
            Value = x.Value
        }).GroupBy(x => new { x.Category, x.CategoryType})
        .Select(x => new CategorySummariesOutput()
        {
            Category = new KeyValuePairModel<Guid, string>() { Key = x.Key.Category.Key, Value = x.Key.Category.Value },
            CategoryType = new KeyValuePairModel<int, string>() { Key = x.Key.CategoryType.Key, Value = x.Key.CategoryType.Value },
            Value = x.Sum(z => z.Value)
        }).ToList();

        return items;
    }

    private record KeyValuePairModelGuidString(Guid Key, string Value);
    private record KeyValuePairModelIntString(int Key, string Value);
}
