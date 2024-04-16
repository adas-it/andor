using Andor.Domain.Communications.Users;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.Mail;

namespace Andor.Infrastructure.Communication.Repositories.Users.Configs;

public record RecipientConfig : IEntityTypeConfiguration<Recipient>
{
    public void Configure(EntityTypeBuilder<Recipient> entity)
    {
        entity.ToTable(nameof(Recipient), SchemasNames.Communication);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(70);
        entity.Property(k => k.PreferredEmail).HasMaxLength(70);

        entity.Property(k => k.Id).HasConversion(
            id => id!.Value,
            value => RecipientId.Load(value));

        entity.Property(k => k.PreferredEmail).HasConversion(
            Email => Email!.Address,
            value => new MailAddress(value));

        entity.HasMany(k => k.Permissions).WithOne(x => x.Recipient).HasForeignKey(x => x.RecipientId);
    }
}
