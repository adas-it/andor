using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> entity)
    {
        _ = entity.ToTable(nameof(Account), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Description)
            .HasConversion(Converters.GetDescriptionConverter())
            .HasMaxLength(Description.MaxLength);

        _ = entity.Property(k => k.IsDeleted);

        _ = entity.Property<CurrencyId>("CurrencyId")
            .HasConversion(id => id.Value, value => CurrencyId.Load(value));

        _ = entity.HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey("CurrencyId")
            .IsRequired();

        _ = entity.HasMany(x => x.Categories)
            .WithOne(x => x.Account!)
            .HasForeignKey(x => x.AccountId);
        _ = entity.Navigation(x => x.Categories).UsePropertyAccessMode(PropertyAccessMode.Field);

        _ = entity.HasMany(x => x.SubCategories)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId);
        _ = entity.Navigation(x => x.SubCategories).UsePropertyAccessMode(PropertyAccessMode.Field);

        _ = entity.HasMany(x => x.PaymentMethods)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId);
        _ = entity.Navigation(x => x.PaymentMethods).UsePropertyAccessMode(PropertyAccessMode.Field);

        _ = entity.HasMany(x => x.Members)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId);
        _ = entity.Navigation(x => x.Members).UsePropertyAccessMode(PropertyAccessMode.Field);

        _ = entity.HasMany(x => x.Invites)
            .WithOne()
            .HasForeignKey(x => x.AccountId);
        _ = entity.Navigation(x => x.Invites).UsePropertyAccessMode(PropertyAccessMode.Field);

        _ = entity.Ignore(x => x.Events);
    }
}
