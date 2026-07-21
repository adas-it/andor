using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class AccountSubCategoryConfig : IEntityTypeConfiguration<AccountSubCategory>
{
    public void Configure(EntityTypeBuilder<AccountSubCategory> entity)
    {
        _ = entity.ToTable(nameof(AccountSubCategory), "Accounts");
        _ = entity.HasKey(x => new { x.AccountId, x.SubCategoryId });

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.SubCategoryId)
            .HasConversion(id => id.Value, value => SubCategoryId.Load(value));

        _ = entity.HasOne(x => x.SubCategory)
            .WithMany()
            .HasForeignKey(x => x.SubCategoryId);
    }
}
