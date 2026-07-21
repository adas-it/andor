using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Andor.Onboarding.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Onboarding.Infrastructure.Config;

public class SignupRequestConfig : IEntityTypeConfiguration<SignupRequest>
{
    public void Configure(EntityTypeBuilder<SignupRequest> entity)
    {
        _ = entity.ToTable(nameof(SignupRequest), "Onboarding");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id!.Value, value => value);

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(x => x.Email).HasMaxLength(320);
        _ = entity.Property(x => x.VerificationCode).HasMaxLength(10);
        _ = entity.Property(x => x.ExpiresAt);
        _ = entity.Property(x => x.IsVerified);
        _ = entity.Property(x => x.CreatedAt);

        _ = entity.Ignore(x => x.Events);
    }
}
