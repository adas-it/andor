using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Engagement.Budget.Categories.Response;

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
