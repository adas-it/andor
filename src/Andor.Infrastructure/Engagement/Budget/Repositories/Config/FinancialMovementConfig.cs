using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record FinancialMovementConfig : IEntityTypeConfiguration<FinancialMovement>
{
    public void Configure(EntityTypeBuilder<FinancialMovement> entity)
    {
        entity.ToTable(nameof(FinancialMovement), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetFinancialMovementIdConverter());
        entity.Property(k => k.AccountId).HasConversion(AccountConfig.GetAccountIdConverter());

        entity.Property(k => k.Value).HasColumnType("decimal(18,2)");
        entity.Property(k => k.Description).HasMaxLength(250);

        entity.Property(k => k.Status).HasConversion(GetMovementStatusConverter());
        entity.Property(k => k.Type).HasConversion(GetMovementTypeConverter());

        entity.HasOne(k => k.Account)
            .WithMany()
            .HasForeignKey(k => k.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static ValueConverter<FinancialMovementId, Guid> GetFinancialMovementIdConverter()
        => new(id => id!.Value, value => FinancialMovementId.Load(value));

    public static ValueConverter<MovementStatus, int> GetMovementStatusConverter()
        => new(id => id!.Key, value => MovementStatus.GetByKey<MovementStatus>(value));

    public static ValueConverter<MovementType, int> GetMovementTypeConverter()
        => new(id => id!.Key, value => MovementType.GetByKey<MovementType>(value));
}
