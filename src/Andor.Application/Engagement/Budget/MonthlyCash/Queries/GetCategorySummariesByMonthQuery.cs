using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.MonthlyCash.Queries;

public record GetCategorySummariesByMonthQuery : IRequest<ApplicationResult<List<CategorySummariesOutput>>>
{
    public AccountId AccountId { get; set; }
    public Year Year { get; set; }
    public Month Month { get; set; }
}

public class GetFinancialSummariesByMonthQueryHandler(IQueriesFinancialMovementRepository _repository)
    : IRequestHandler<GetCategorySummariesByMonthQuery, ApplicationResult<List<CategorySummariesOutput>>>
{

    public async Task<ApplicationResult<List<CategorySummariesOutput>>> Handle(GetCategorySummariesByMonthQuery request, CancellationToken cancellationToken)
    {
        var listFinancialMovements = await _repository.GetAllFinancialMovementsByMonth(
            request.AccountId,
            request.Year,
            request.Month,
            cancellationToken);

        var items = listFinancialMovements.Select(x => new
        {
            Category = new KeyValuePair<Guid, string>(x.SubCategory.Category.Id, x.SubCategory.Category.Name),
            CategoryType = new KeyValuePair<int, string>(x.SubCategory.Category.Type.Key, x.SubCategory.Category.Type.Name),
            Value = x.Value
        }).GroupBy(x => new { x.Category, x.CategoryType })
        .Select(x => new CategorySummariesOutput()
        {
            Category = new KeyValuePair<Guid, string>(x.Key.Category.Key, x.Key.Category.Value),
            CategoryType = new KeyValuePair<int, string>(x.Key.CategoryType.Key, x.Key.CategoryType.Value),
            Value = x.Sum(z => z.Value)
        }).ToList();

        return items;
    }
}
