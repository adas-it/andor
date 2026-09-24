using Andor.Communications.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Communications.Infrastructure.Config;

public class RecipientConfig : IEntityTypeConfiguration<Recipient>
{
    public void Configure(EntityTypeBuilder<Recipient> entity)
    {
        _ = entity.ToTable(nameof(Recipient), "Communication");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => value);

        _ = entity.Property(x => x.Name).HasMaxLength(50);
        _ = entity.Property(x => x.Email).HasMaxLength(255);
        _ = entity.Property(x => x.PreferredLanguageId);
        _ = entity.Property(x => x.Active);
        _ = entity.Property(x => x.MarketingOptIn);
        _ = entity.Property(x => x.TermsAndConditionsAccepted);
        _ = entity.Property(x => x.PrivacyPolicyAccepted);
    }
}
