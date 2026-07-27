using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Contracts.SubCategories.Responses;

public record ListSubCategoriesOutput
    : PaginatedListOutput<SubCategoryOutput>
{
    public ListSubCategoriesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<SubCategoryOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
