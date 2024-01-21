
namespace Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Dto.Models.Response;

public record ListCurrenciesOutput
    : PaginatedListOutput<CurrencyOutput>
{
    public ListCurrenciesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<CurrencyOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
