using Andor.Accounts.Domain.CashFlows;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class CashFlowAppliedMovementConfig : IEntityTypeConfiguration<CashFlowAppliedMovement>
{
    public void Configure(EntityTypeBuilder<CashFlowAppliedMovement> entity)
    {
        _ = entity.ToTable(nameof(CashFlowAppliedMovement), "Accounts");
        _ = entity.HasKey(k => k.FinancialMovementId);

        _ = entity.Property(k => k.FinancialMovementId)
            .HasConversion(id => id.Value, value => FinancialMovementId.Load(value));

        _ = entity.Property(k => k.CashFlowId)
            .HasConversion(id => id.Value, value => CashFlowId.Load(value));

        _ = entity.Property(k => k.AppliedOn);
    }
}
