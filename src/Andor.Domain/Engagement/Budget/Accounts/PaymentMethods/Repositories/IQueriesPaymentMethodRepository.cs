using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.Repositories;

public interface IQueriesPaymentMethodRepository :
    IResearchableRepository<PaymentMethod, PaymentMethodId, SearchInput>
{
    Task<List<PaymentMethod>> GetManyByIdsAsync(List<PaymentMethodId> ids, CancellationToken cancellationToken);
}
