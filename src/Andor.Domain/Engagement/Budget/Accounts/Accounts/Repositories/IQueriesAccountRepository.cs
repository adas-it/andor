using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface IQueriesAccountRepository :
    IResearchableRepository<Account, AccountId, SearchInput>
{
}
