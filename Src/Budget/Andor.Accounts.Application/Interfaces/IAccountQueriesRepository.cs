using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Application.Queries;

namespace Andor.Accounts.Application.Interfaces;

public interface IAccountQueriesRepository :
    ISearchableRepository<Account, AccountId, SearchInput>
{
}
