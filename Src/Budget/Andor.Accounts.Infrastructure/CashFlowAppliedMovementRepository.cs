using Andor.Accounts.Domain.CashFlows;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CashFlowAppliedMovementRepository(AccountsContext context) : ICashFlowAppliedMovementRepository
{
    protected readonly DbSet<CashFlowAppliedMovement> DbSet = context.Set<CashFlowAppliedMovement>();

    public Task<bool> HasBeenAppliedAsync(FinancialMovementId financialMovementId, CancellationToken cancellationToken)
    {
        var applied = DbSet.Any(x => x.FinancialMovementId == financialMovementId);

        return Task.FromResult(applied);
    }

    public async Task MarkAppliedAsync(FinancialMovementId financialMovementId, CashFlowId cashFlowId, CancellationToken cancellationToken)
    {
        DbSet.Add(CashFlowAppliedMovement.New(financialMovementId, cashFlowId));

        await context.SaveChangesAsync(cancellationToken);
    }
}
