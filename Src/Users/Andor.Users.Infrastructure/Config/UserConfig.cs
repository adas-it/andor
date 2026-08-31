using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Andor.Users.Domain.Users;

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
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(320);

        _ = entity.Property(k => k.PreferredCurrencyId);

        _ = entity.Property(k => k.PreferredLanguageId);
    }
}
