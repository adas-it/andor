using Andor.Application.Dto.Common.Responses;

namespace Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;

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
