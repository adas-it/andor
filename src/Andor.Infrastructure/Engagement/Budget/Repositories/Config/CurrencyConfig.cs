using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record CurrencyConfig : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> entity)
    {
        entity.ToTable(nameof(Currency), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetCurrencyIdConverter());
        entity.Property(k => k.Name).HasMaxLength(70);
        entity.Property(k => k.Iso).HasMaxLength(Iso.MaxLength);
    }

    public static ValueConverter<CurrencyId, Guid> GetCurrencyIdConverter()
            => new(id => id!.Value, value => CurrencyId.Load(value));
}