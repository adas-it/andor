namespace Family.Budget.Infrastructure.Repositories.PaymentMethod;

using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> entity)
    {
        entity.ToTable(nameof(PaymentMethod), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Description).HasMaxLength(300);
        entity.Ignore(k => k.Events);

        var typeConverter = new ValueConverter<MovementType, int>(
        v => v.Key,
        v => MovementType.GetByKey<MovementType>(v)!);

        entity.Property(K => K.Type).HasConversion(typeConverter);
    }
}