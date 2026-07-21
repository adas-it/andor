using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class AccountCategoryConfig : IEntityTypeConfiguration<AccountCategory>
{
    public void Configure(EntityTypeBuilder<AccountCategory> entity)
    {
        _ = entity.ToTable(nameof(AccountCategory), "Accounts");
        _ = entity.HasKey(x => new { x.AccountId, x.CategoryId });

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.CategoryId)
            .HasConversion(id => id.Value, value => CategoryId.Load(value));

        _ = entity.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
    }
}
