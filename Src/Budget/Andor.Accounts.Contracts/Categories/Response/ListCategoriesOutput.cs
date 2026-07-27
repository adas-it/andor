using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Contracts.Categories.Response;

public record ListCategoriesOutput
    : PaginatedListOutput<CategoryOutput>
{
    public ListCategoriesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<CategoryOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
