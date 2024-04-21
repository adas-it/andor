using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;

public record ListFinancialMovementsOutput
    : PaginatedListOutput<FinancialMovementOutput>
{
    public ListFinancialMovementsOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<FinancialMovementOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
