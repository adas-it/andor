using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Andor.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Users.Infrastructure.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        _ = entity.ToTable(nameof(User), "Users");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => value);

        _ = entity.Property(k => k.FirstName)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.LastName)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Email)
            .HasConversion(Converters.GetEmailConverter())
            .HasMaxLength(Email.MaxLength);

        _ = entity.Property(k => k.PreferredCurrencyId);

        _ = entity.Property(k => k.PreferredLanguageId);

        _ = entity.Property(k => k.MarketingOptIn);

        _ = entity.Property(k => k.TermsAndConditionsAccepted);

        _ = entity.Property(k => k.PrivacyPolicyAccepted);
    }
}
