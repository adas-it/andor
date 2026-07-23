using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Contracts.Responses;

public record ListAccountOutput
    : PaginatedListOutput<AccountOutput>
{
    public ListAccountOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<AccountOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
