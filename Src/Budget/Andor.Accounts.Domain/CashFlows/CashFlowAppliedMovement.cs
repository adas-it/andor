using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;

namespace Andor.Accounts.Domain.CashFlows;

/// <summary>
/// Idempotency ledger for the CashFlow projection consumer. Unlike persisting a
/// FinancialMovement (an upsert-by-id, naturally idempotent), applying a movement to a
/// CashFlow row is an incremental update — reprocessing the same Service Bus message twice
/// would double-count its value without this guard.
/// </summary>
public class CashFlowAppliedMovement
{
    public FinancialMovementId FinancialMovementId { get; private set; }
    public CashFlowId CashFlowId { get; private set; }
    public DateTimeOffset AppliedOn { get; private set; }

    protected CashFlowAppliedMovement()
    {
    }

    private CashFlowAppliedMovement(FinancialMovementId financialMovementId, CashFlowId cashFlowId, DateTimeOffset appliedOn)
    {
        FinancialMovementId = financialMovementId;
        CashFlowId = cashFlowId;
        AppliedOn = appliedOn;
    }

    public static CashFlowAppliedMovement New(FinancialMovementId financialMovementId, CashFlowId cashFlowId)
        => new(financialMovementId, cashFlowId, DateTimeOffset.UtcNow);
}
