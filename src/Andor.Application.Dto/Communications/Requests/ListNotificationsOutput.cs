using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Communications.Requests;

public record ListNotificationsOutput
    : PaginatedListOutput<NotificationsOutput>
{
    public ListNotificationsOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<NotificationsOutput> items)
        : base(page, perPage, total, items)
    {
    }
}

