using Andor.Domain.Engagement.Budget.Entities.MovementTypes;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> entity)
    {
        entity.ToTable(nameof(PaymentMethod), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetPaymentMethodIdConverter());
        entity.Property(k => k.Description).HasMaxLength(255);

        entity.Property(k => k.Type).HasConversion(
            State => State.Key,
            value => MovementType.GetByKey<MovementType>(value));
    }

    public static ValueConverter<PaymentMethodId, Guid> GetPaymentMethodIdConverter()
        => new(id => id!.Value, value => PaymentMethodId.Load(value));
}