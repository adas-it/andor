using Andor.Configurations.Domain;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Configurations.Infrastructure.Config;

public class ConfigurationProjectionConfig : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> entity)
    {
        _ = entity.ToTable(nameof(Configuration), "Administration");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id!.Value, value => value);

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Value)
            .HasConversion(Converters.GetValueConverter())
            .HasMaxLength(Value.MaxLength);

        _ = entity.Property(k => k.Description)
            .HasConversion(Converters.GetDescriptionConverter())
            .HasMaxLength(Description.MaxLength);

        _ = entity.Property(k => k.Type)
            .HasConversion(id => id!.Key, value => ConfigurationType.GetByKey<ConfigurationType>(value))
            .HasMaxLength(Description.MaxLength);

        _ = entity.Ignore(k => k.State);

        _ = entity.Property(x => x.StartDate);

        _ = entity.Property(x => x.ExpireDate);

        _ = entity.Property(x => x.DeletedDate);
    }
}
