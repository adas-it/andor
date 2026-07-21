using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class CurrencyConfig : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> entity)
    {
        _ = entity.ToTable(nameof(Currency), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => CurrencyId.Load(value));

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Iso).HasMaxLength(3);
        _ = entity.Property(k => k.Symbol).HasMaxLength(5);
    }
}
