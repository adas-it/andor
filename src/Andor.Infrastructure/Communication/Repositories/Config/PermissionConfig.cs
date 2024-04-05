using Andor.Domain.Entities.Communications;
using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects = Andor.Domain.Entities.Communications.ValueObjects;

namespace Andor.Infrastructure.Communication.Repositories.Config;

public record PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> entity)
    {
        entity.ToTable(nameof(Permission), SchemasNames.Communication);
        entity.HasKey(k => k.Id);

        entity.Property(k => k.Id)
        .HasConversion(
            id => id!.Value,
            value => PermissionId.Load(value)
        );

        entity.Property(k => k.Type)
        .HasConversion(
            State => State.Key,
            value => ValueObjects.Type.GetByKey<ValueObjects.Type>(value)
        );

        entity.HasOne(k => k.Recipient).WithMany(x => x.Permissions).HasForeignKey(x => x.RecipientId);
    }
}
