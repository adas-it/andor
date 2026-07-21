using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.FinancialMovements.Repositories;

public interface ICommandsFinancialMovementRepository : ICommandRepository<FinancialMovement, FinancialMovementId>
{
}
