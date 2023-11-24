
namespace Family.Budget.Application.Dto.Accounts.Responses;
using Family.Budget.Application.Dto.Models.Response;

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
