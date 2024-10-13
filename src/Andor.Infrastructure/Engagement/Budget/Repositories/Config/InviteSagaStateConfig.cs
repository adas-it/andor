using Andor.Application.Engagement.Budget.Invites.Saga;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record InviteSagaStateConfig : IEntityTypeConfiguration<InviteSagaState>
{
    public void Configure(EntityTypeBuilder<InviteSagaState> entity)
    {
        entity.ToTable(nameof(InviteSagaState), SchemasNames.Engagement);
        entity.HasKey(k => k.CorrelationId);

        entity.Property(k => k.InviteId).HasConversion(InviteConfig.GetInviteIdConverter());
        entity.Property(k => k.GuestId).HasConversion(UserConfig.GetUserIdConverter());
        entity.Property(k => k.InvitingId).HasConversion(UserConfig.GetUserIdConverter());
    }
}