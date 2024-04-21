using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Currencies.Repositories;

public interface IQueriesCurrencyRepository :
    IResearchableRepository<Currency, CurrencyId, SearchInput>
{
}
