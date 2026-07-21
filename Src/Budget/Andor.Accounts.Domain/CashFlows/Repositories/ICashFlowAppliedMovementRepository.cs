using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;

namespace Andor.Accounts.Domain.CashFlows.Repositories;

public interface ICashFlowAppliedMovementRepository
{
    Task<bool> HasBeenAppliedAsync(FinancialMovementId financialMovementId, CancellationToken cancellationToken);

    Task MarkAppliedAsync(FinancialMovementId financialMovementId, CashFlowId cashFlowId, CancellationToken cancellationToken);
}
