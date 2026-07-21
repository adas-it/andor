using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class AccountUserConfig : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> entity)
    {
        _ = entity.ToTable(nameof(AccountUser), "Accounts");
        _ = entity.HasKey(x => new { x.AccountId, x.UserId });

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.PermissionType)
            .HasConversion(id => id.Key, value => PermissionType.GetByKey<PermissionType>(value));

        // "User" here is just a lightweight reference to a Guid identity owned by the Identity
        // bounded context, not an aggregate Accounts persists itself — no FK/table for it.
        _ = entity.Ignore(x => x.User);
    }
}
