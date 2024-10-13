using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Engagement.Budget.Invites.Responses;

public record ListInviteOutput
    : PaginatedListOutput<InviteOutput>
{
    public ListInviteOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<InviteOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
