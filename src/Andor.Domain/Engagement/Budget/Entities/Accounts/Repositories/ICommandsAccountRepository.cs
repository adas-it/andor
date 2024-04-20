using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts.Repositories;

public interface ICommandsAccountRepository : ICommandRepository<Account, AccountId>
{
}
