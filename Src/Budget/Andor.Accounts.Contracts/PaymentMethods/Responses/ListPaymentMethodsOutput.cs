using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Contracts.PaymentMethods.Responses;

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
