
namespace Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Dto.Models.Response;

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
