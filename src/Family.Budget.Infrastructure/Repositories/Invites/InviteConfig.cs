using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.SubCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Family.Budget.Infrastructure.Repositories.Invites;

public class InviteConfig : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> entity)
    {
        var statusConverter = new ValueConverter<InviteStatus, int>(
        v => v.Key,
        v => InviteStatus.GetByKey<InviteStatus>(v));

        var accountIdConverter = new ValueConverter<AccountId, Guid>(
        v => v.Value,
        v => (AccountId)v);

        entity.ToTable(nameof(Invite), SchemasNames.FamilyBudget);

        entity.HasKey(k => k.Id);
        entity.Property(K => K.Status).HasConversion(statusConverter);
        entity.Property(K => K.AccountId).HasConversion(accountIdConverter);
        entity.Ignore(x => x.Events);

        entity.HasOne(x => x.Account).WithMany(x => x.Invites).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}