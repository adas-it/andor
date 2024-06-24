using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsFinancialMovementRepository(PrincipalContext context) :
    CommandsBaseRepository<FinancialMovement, FinancialMovementId>(context),
    ICommandsFinancialMovementRepository
{
}
