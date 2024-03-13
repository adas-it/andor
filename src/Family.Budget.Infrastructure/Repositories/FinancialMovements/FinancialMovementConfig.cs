namespace Family.Budget.Infrastructure.Repositories.FinancialMovements;

using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class FinancialMovementConfig : IEntityTypeConfiguration<FinancialMovement>
{
    public void Configure(EntityTypeBuilder<FinancialMovement> entity)
    {
        var statusConverter = new ValueConverter<MovementStatus, int>(
        v => v.Key,
        v => MovementStatus.GetByKey<MovementStatus>(v));
        
        var typeConverter = new ValueConverter<MovementType, int>(
        v => v.Key,
        v => MovementType.GetByKey<MovementType>(v));

        entity.ToTable(nameof(FinancialMovement), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(K => K.Status).HasConversion(statusConverter);
        entity.Property(K => K.Type).HasConversion(typeConverter);
        entity.Property(k => k.Value);
        entity.Ignore(x => x.Events);

        entity.Navigation(x => x.SubCategory).AutoInclude();
        entity.Navigation(x => x.PaymentMethod).AutoInclude();
    }
}
