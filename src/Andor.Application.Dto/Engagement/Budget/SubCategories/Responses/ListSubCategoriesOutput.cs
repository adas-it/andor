using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;

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
