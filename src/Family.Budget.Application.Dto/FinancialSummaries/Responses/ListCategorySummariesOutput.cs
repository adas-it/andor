namespace Family.Budget.Application.Dto.FinancialSummaries.Responses;

using Family.Budget.Application.Dto.Models.Response;
using System.Collections.Generic;
public record ListCategorySummariesOutput
    : PaginatedListOutput<CategorySummariesOutput>
{
    public ListCategorySummariesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<CategorySummariesOutput> items)
        : base(page, perPage, total, items)
    {
    }
}