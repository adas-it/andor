using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            value => Domain.Communications.ValueObjects.Type.GetByKey<Domain.Communications.ValueObjects.Type>(value)
        );

        entity.HasOne(k => k.Recipient).WithMany(x => x.Permissions).HasForeignKey(x => x.RecipientId);
    }
}
