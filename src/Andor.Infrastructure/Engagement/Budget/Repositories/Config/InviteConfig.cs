using Andor.Domain.Engagement.Budget.Entities.Invites;
using Andor.Domain.Engagement.Budget.Entities.Invites.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net.Mail;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record InviteConfig : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> entity)
    {
        entity.ToTable(nameof(Invite), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetInviteIdConverter());
        entity.Property(k => k.Email)
            .HasMaxLength(70)
            .HasConversion(
                Email => Email!.Address,
                value => new MailAddress(value));

        entity.Property(k => k.Status).HasConversion(
            State => State.Key,
            value => InviteStatus.GetByKey<InviteStatus>(value));

        entity.HasOne(k => k.Account).WithMany(x => x.Invites).HasForeignKey(x => x.AccountId);
    }

    public static ValueConverter<InviteId, Guid> GetInviteIdConverter()
            => new(id => id!.Value, value => InviteId.Load(value));
}