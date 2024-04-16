using Andor.Domain.Entities.Admin.Configurations;
using Andor.Domain.Entities.Admin.Configurations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Infrastructure.Administrations.Repositories.Configurations.Config;

public record ConfigurationConfig : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> entity)
    {
        entity.ToTable(nameof(Configuration), SchemasNames.Administration);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(70);
        entity.Property(k => k.Value).HasMaxLength(2500);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Property(k => k.CreatedBy).HasMaxLength(70);
        entity.Ignore(k => k.Events);
        entity.Ignore(k => k.State);

        entity.Property(k => k.Id)
        .HasConversion(
            id => id!.Value,
            value => ConfigurationId.Load(value)
        );
    }
}
