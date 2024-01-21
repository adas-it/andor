namespace Family.Budget.Infrastructure.Repositories.Currencies;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class CurrencyConfig : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> entity)
    {
        entity.ToTable(nameof(Currency), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Iso).HasMaxLength(3);
        entity.Ignore(k => k.Events);
    }
}