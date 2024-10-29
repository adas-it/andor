using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesAccountPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(AccountId accountId, PaymentMethodId paymentMethodId, CancellationToken cancellationToken);

    Task<ListPaymentMethodsOutput> SearchAsync(SearchInputAccountPayment input, CancellationToken cancellationToken);
}
