using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.PaymentMethods.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public interface IAccountPaymentMethodQueriesService
{
    Task<ApplicationResult<ListPaymentMethodsOutput?>> GetByAccountIdAndPaymentMethodAsync(ListPaymentMethodsQuery query, CancellationToken cancellationToken);
    Task<ApplicationResult<PaymentMethodOutput?>> GetByAccountIdAndPaymentMethodIdAsync(AccountId accountId, PaymentMethodId paymentMethodId, CancellationToken cancellationToken);
}
