namespace Family.Budget.Infrastructure.Repositories.SubCategories;

using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> entity)
    {
        var AccountIdConverter = new ValueConverter<AccountId, Guid>(
        v => v.Value,
        v => (AccountId)v);

        entity.ToTable(nameof(Account), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Ignore(x => x.Events);

        entity.HasMany(k => k.Categories).WithOne(x => x.Account).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        entity.HasMany(k => k.SubCategories).WithOne(x => x.Account).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        entity.HasMany(k => k.PaymentMethods).WithOne(x => x.Account).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        entity.HasMany(k => k.UserIds).WithOne(x => x.Account).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        entity.HasMany(k => k.Invites).WithOne(x => x.Account).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.Navigation(x => x.Invites).AutoInclude();
    }
}

public class AccountCategoryConfig : IEntityTypeConfiguration<AccountCategory>
{
    public void Configure(EntityTypeBuilder<AccountCategory> entity)
    {
        entity.ToTable(nameof(AccountCategory), SchemasNames.FamilyBudget);
        entity.HasKey(x => new {x.AccountId, x.CategoryId });

        entity.HasOne(k => k.Account).WithMany(x => x.Categories);
        entity.HasOne(k => k.Category);
    }
}
public class AccountSubCategoryConfig : IEntityTypeConfiguration<AccountSubCategory>
{
    public void Configure(EntityTypeBuilder<AccountSubCategory> entity)
    {
        entity.ToTable(nameof(AccountSubCategory), SchemasNames.FamilyBudget);
        entity.HasKey(x => new { x.AccountId, x.SubCategoryId });

        entity.HasOne(k => k.Account).WithMany(x => x.SubCategories);
        entity.HasOne(k => k.SubCategory);
    }
}
public class AccountPaymentMethodConfig : IEntityTypeConfiguration<AccountPaymentMethod>
{
    public void Configure(EntityTypeBuilder<AccountPaymentMethod> entity)
    {
        entity.ToTable(nameof(AccountPaymentMethod), SchemasNames.FamilyBudget);
        entity.HasKey(x => new { x.AccountId, x.PaymentMethodId });

        entity.HasOne(k => k.Account).WithMany(x => x.PaymentMethods);
        entity.HasOne(k => k.PaymentMethod);
    }
}
public class AccountUserConfig : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> entity)
    {
        entity.ToTable(nameof(AccountUser), SchemasNames.FamilyBudget);
        entity.HasKey(x => new { x.AccountId, x.UserId });
    }
}
