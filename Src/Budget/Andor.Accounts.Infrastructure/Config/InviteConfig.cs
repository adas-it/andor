using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Invites;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class InviteConfig : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> entity)
    {
        _ = entity.ToTable(nameof(Invite), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => InviteId.Load(value));

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.UserId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value.Value,
                value => value == null ? (UserId?)null : UserId.Load(value.Value));

        _ = entity.Property(k => k.Email)
            .HasConversion(
                id => id == null ? null : id.Value,
                value => value == null ? null : Email.Create(value));

        _ = entity.Property(k => k.Permission)
            .HasConversion(id => id.Key, value => PermissionType.GetByKey<PermissionType>(value));

        _ = entity.Property(k => k.IsActive);
        _ = entity.Property(k => k.IsAccepted);
    }
}
