using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface ICommandsFinancialMovementRepository : ICommandRepository<FinancialMovement, FinancialMovementId>
{
}
