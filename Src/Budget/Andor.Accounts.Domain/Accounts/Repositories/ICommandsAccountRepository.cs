using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.Accounts.Repositories;

public interface ICommandsAccountRepository : ICommandRepository<Account, AccountId>
{
    Task UpsertFinancialMovement(FinancialMovement entity, CancellationToken cancellationToken);

    Task<FinancialMovement?> GetFinancialMovementByIdAsync(FinancialMovementId id, CancellationToken cancellationToken);
}
