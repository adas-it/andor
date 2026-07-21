using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> entity)
    {
        _ = entity.ToTable(nameof(PaymentMethod), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => PaymentMethodId.Load(value));

        _ = entity.Property(k => k.Owner)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value.Value,
                value => value == null ? (AccountId?)null : AccountId.Load(value.Value));

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Description)
            .HasConversion(Converters.GetDescriptionConverter())
            .HasMaxLength(Description.MaxLength);

        _ = entity.Property(k => k.Type)
            .HasConversion(id => id.Key, value => MovementType.GetByKey<MovementType>(value));

        _ = entity.Property(k => k.IsDeleted);
    }
}
