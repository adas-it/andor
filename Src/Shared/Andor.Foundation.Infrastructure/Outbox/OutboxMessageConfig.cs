using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Foundation.Infrastructure.Outbox;

public sealed class OutboxMessageConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> entity)
    {
        _ = entity.ToTable("OutboxMessages", "Outbox");

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

        // Optimizes the dispatcher query that scans for pending messages in order.
        _ = entity.HasIndex(k => new { k.ProcessedOn, k.OccurredOn })
            .HasDatabaseName("IX_OutboxMessages_ProcessedOn_OccurredOn");
    }
}
