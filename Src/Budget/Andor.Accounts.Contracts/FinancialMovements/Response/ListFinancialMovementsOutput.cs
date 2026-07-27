using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Contracts.FinancialMovements.Response;

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
