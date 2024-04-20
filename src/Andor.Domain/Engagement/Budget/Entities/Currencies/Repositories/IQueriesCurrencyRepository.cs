using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface IQueriesCurrencyRepository :
    IResearchableRepository<Currency, CurrencyId, SearchInput>
{
}
