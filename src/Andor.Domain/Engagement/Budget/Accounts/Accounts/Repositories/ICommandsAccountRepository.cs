using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface ICommandsAccountRepository : ICommandRepository<Account, AccountId>
{
}
