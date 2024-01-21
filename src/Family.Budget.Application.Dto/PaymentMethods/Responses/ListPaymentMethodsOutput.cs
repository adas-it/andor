
namespace Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Dto.Models.Response;

public record ListPaymentMethodsOutput
    : PaginatedListOutput<PaymentMethodOutput>
{
    public ListPaymentMethodsOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<PaymentMethodOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
