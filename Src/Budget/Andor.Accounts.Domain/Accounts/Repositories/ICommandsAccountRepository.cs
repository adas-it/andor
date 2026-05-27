using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.Accounts.Repositories;

public interface ICommandsAccountRepository : ICommandRepository<Account, AccountId>
{
}
