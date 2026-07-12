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
        entity.ToTable(nameof(Configuration), "Administration");
        entity.HasKey(k => k.Id);

        entity.Property(k => k.Id)
            .HasConversion(id => id!.Value, value => value);

        entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        entity.Property(k => k.Value)
            .HasConversion(Converters.GetValueConverter())
            .HasMaxLength(Value.MaxLength);

        entity.Property(k => k.Description)
            .HasConversion(Converters.GetDescriptionConverter())
            .HasMaxLength(Description.MaxLength);

        entity.Property(k => k.Type)
            .HasConversion(id => id!.Key, value => ConfigurationType.GetByKey<ConfigurationType>(value))
            .HasMaxLength(Description.MaxLength);

        entity.Ignore(k => k.State);

        entity.Property(x => x.StartDate)
            .HasColumnType("timestamp with time zone");

        entity.Property(x => x.ExpireDate)
            .HasColumnType("timestamp with time zone");

        entity.Property(x => x.DeletedDate)
            .HasColumnType("timestamp with time zone");
    }
}
