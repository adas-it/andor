using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

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
