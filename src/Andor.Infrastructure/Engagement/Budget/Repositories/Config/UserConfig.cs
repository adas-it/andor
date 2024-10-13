using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Users;
using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net.Mail;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable(nameof(User), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetUserIdConverter());
        entity.Property(k => k.Email)
            .HasMaxLength(70)
            .HasConversion(
                Email => Email!.Address,
                value => new MailAddress(value));

        entity.Property(k => k.PreferredCurrencyId)
        .HasConversion(
            id => id!.Value,
            value => CurrencyId.Load(value)
        );

        entity.Property(k => k.PreferredLanguageId)
        .HasConversion(
            id => id!.Value,
            value => LanguageId.Load(value)
        );
    }

    public static ValueConverter<UserId, Guid> GetUserIdConverter()
            => new(id => id!.Value, value => UserId.Load(value));
}