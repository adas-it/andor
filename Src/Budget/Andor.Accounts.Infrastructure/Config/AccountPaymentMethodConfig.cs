using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class AccountPaymentMethodConfig : IEntityTypeConfiguration<AccountPaymentMethod>
{
    public void Configure(EntityTypeBuilder<AccountPaymentMethod> entity)
    {
        _ = entity.ToTable(nameof(AccountPaymentMethod), "Accounts");
        _ = entity.HasKey(x => new { x.AccountId, x.PaymentMethodId });

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.PaymentMethodId)
            .HasConversion(id => id.Value, value => PaymentMethodId.Load(value));

        _ = entity.HasOne(x => x.PaymentMethod)
            .WithMany()
            .HasForeignKey(x => x.PaymentMethodId);
    }
}
