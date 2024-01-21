namespace Family.Budget.Application.Dto.FinancialMovements.Responses;

using Family.Budget.Application.Dto.Models.Response;

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
