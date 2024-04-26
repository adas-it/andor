using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Onboarding.Users.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> entity)
    {
        entity.ToTable(nameof(Account), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetAccountIdConverter());
        entity.Property(k => k.Name).HasMaxLength(70);
        entity.Property(k => k.Description).HasMaxLength(70);

        entity.Ignore(x => x.Events);

        entity.HasMany(k => k.Categories).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);
        entity.HasMany(k => k.SubCategories).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);
        entity.HasMany(k => k.PaymentMethods).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);
        entity.HasMany(k => k.Users).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);
        entity.HasMany(k => k.Invites).WithOne(x => x.Account).HasForeignKey(x => x.AccountId);

        entity.Navigation(x => x.Invites).AutoInclude();
    }

    public static ValueConverter<AccountId, Guid> GetAccountIdConverter()
        => new(id => id!.Value, value => AccountId.Load(value));
}

public class AccountCategoryConfig : IEntityTypeConfiguration<AccountCategory>
{
    public void Configure(EntityTypeBuilder<AccountCategory> entity)
    {
        entity.ToTable(nameof(AccountCategory), SchemasNames.Engagement);
        entity.HasKey(x => new { x.AccountId, x.CategoryId });

        entity.Property(x => x.AccountId).HasConversion(AccountConfig.GetAccountIdConverter());
        entity.Property(x => x.CategoryId).HasConversion(CategoryConfig.GetCategoryIdConverter());

        entity.HasOne(k => k.Account).WithMany(x => x.Categories).HasForeignKey(x => x.AccountId);
        entity.HasOne(k => k.Category);

        entity.Navigation(x => x.Category).AutoInclude();
        entity.Navigation(x => x.Account).AutoInclude();
    }
}
public class AccountSubCategoryConfig : IEntityTypeConfiguration<AccountSubCategory>
{
    public void Configure(EntityTypeBuilder<AccountSubCategory> entity)
    {
        entity.ToTable(nameof(AccountSubCategory), SchemasNames.Engagement);
        entity.HasKey(x => new { x.AccountId, x.SubCategoryId });

        entity.Property(x => x.AccountId).HasConversion(AccountConfig.GetAccountIdConverter());
        entity.Property(x => x.SubCategoryId).HasConversion(SubCategoryConfig.GetSubCategoryIdConverter());

        entity.HasOne(k => k.Account).WithMany(x => x.SubCategories).HasForeignKey(x => x.AccountId);
        entity.HasOne(k => k.SubCategory);

        entity.Navigation(x => x.SubCategory).AutoInclude();
        entity.Navigation(x => x.Account).AutoInclude();
    }
}
public class AccountPaymentMethodConfig : IEntityTypeConfiguration<AccountPaymentMethod>
{
    public void Configure(EntityTypeBuilder<AccountPaymentMethod> entity)
    {
        entity.ToTable(nameof(AccountPaymentMethod), SchemasNames.Engagement);
        entity.HasKey(x => new { x.AccountId, x.PaymentMethodId });

        entity.Property(x => x.AccountId).HasConversion(AccountConfig.GetAccountIdConverter());
        entity.Property(x => x.PaymentMethodId).HasConversion(PaymentMethodConfig.GetPaymentMethodIdConverter());

        entity.HasOne(k => k.Account).WithMany(x => x.PaymentMethods).HasForeignKey(x => x.AccountId);
        entity.HasOne(k => k.PaymentMethod);
    }
}
public class AccountUserConfig : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> entity)
    {
        entity.ToTable(nameof(AccountUser), SchemasNames.Engagement);
        entity.HasKey(x => new { x.AccountId, x.UserId });

        entity.Property(x => x.AccountId).HasConversion(AccountConfig.GetAccountIdConverter());
        entity.Property(x => x.UserId).HasConversion(GetUserIdConverter());
    }

    public static ValueConverter<UserId, Guid> GetUserIdConverter()
            => new(id => id!.Value, value => UserId.Load(value));
}
