
namespace Family.Budget.Application.Dto.SubCategories.Responses;
using Family.Budget.Application.Dto.Models.Response;

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
