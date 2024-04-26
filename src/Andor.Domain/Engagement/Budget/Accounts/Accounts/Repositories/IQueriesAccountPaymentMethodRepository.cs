using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesAccountPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(AccountId accountId, PaymentMethodId paymentMethodId, CancellationToken cancellationToken);

    Task<SearchOutput<PaymentMethod>> SearchAsync(SearchInputAccountPayment input, CancellationToken cancellationToken);
}
