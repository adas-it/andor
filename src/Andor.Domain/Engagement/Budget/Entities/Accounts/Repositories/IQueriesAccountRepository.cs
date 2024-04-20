using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts.Repositories;

public interface IQueriesAccountRepository :
    IResearchableRepository<Account, AccountId, SearchInput>
{
}
