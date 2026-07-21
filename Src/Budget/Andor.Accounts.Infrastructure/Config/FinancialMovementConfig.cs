using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class FinancialMovementConfig : IEntityTypeConfiguration<FinancialMovement>
{
    public void Configure(EntityTypeBuilder<FinancialMovement> entity)
    {
        _ = entity.ToTable(nameof(FinancialMovement), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => FinancialMovementId.Load(value));

        _ = entity.Property(k => k.Date);

        _ = entity.Property(k => k.Description)
            .HasMaxLength(250);

        _ = entity.Property(k => k.SubCategoryId)
            .HasConversion(id => id.Value, value => SubCategoryId.Load(value));

        _ = entity.HasOne(x => x.SubCategory)
            .WithMany()
            .HasForeignKey(x => x.SubCategoryId)
            .IsRequired();

        _ = entity.Property(k => k.PaymentMethodId)
            .HasConversion(id => id.Value, value => PaymentMethodId.Load(value));

        _ = entity.HasOne(x => x.PaymentMethod)
            .WithMany()
            .HasForeignKey(x => x.PaymentMethodId)
            .IsRequired();

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .IsRequired();

        _ = entity.Property(k => k.Value)
            .HasColumnType("decimal(18,2)");

        _ = entity.Property(k => k.Status)
            .HasConversion(id => id.Key, value => MovementStatus.GetByKey<MovementStatus>(value));

        _ = entity.Property(k => k.IsDeleted);

        _ = entity.Ignore(x => x.Type);
    }
}
