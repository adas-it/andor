using Andor.Domain.Engagement.Budget.Entities.PaymentMethods;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface IQueriesPaymentMethodRepository :
    IResearchableRepository<PaymentMethod, PaymentMethodId, SearchInput>
{
    Task<List<PaymentMethod>> GetManyByIdsAsync(List<PaymentMethodId> ids, CancellationToken cancellationToken);
}
