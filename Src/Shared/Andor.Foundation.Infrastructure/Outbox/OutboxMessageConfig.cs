using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Foundation.Infrastructure.Outbox;

/// <summary>
/// Maps <see cref="OutboxMessage"/> into a schema owned by the calling module, since every
/// module's <see cref="PrincipalContext"/>-derived context may run its migrations against the
/// same physical database — a shared "Outbox" schema would collide across modules.
/// </summary>
public sealed class OutboxMessageConfig(string schema) : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> entity)
    {
        _ = entity.ToTable("OutboxMessages", schema);

        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Type)
            .HasMaxLength(512)
            .IsRequired();

        _ = entity.Property(k => k.Content)
            .IsRequired();

        _ = entity.Property(k => k.OccurredOn)
            .IsRequired();

        _ = entity.Property(k => k.ProcessedOn);

        _ = entity.Property(k => k.Attempts)
            .IsRequired();

        _ = entity.Property(k => k.Error)
            .HasMaxLength(2048);

        _ = entity.Property(k => k.TargetQueue)
            .HasMaxLength(256);

        // Optimizes the dispatcher query that scans for pending messages in order.
        _ = entity.HasIndex(k => new { k.ProcessedOn, k.OccurredOn })
            .HasDatabaseName("IX_OutboxMessages_ProcessedOn_OccurredOn");
    }
}
