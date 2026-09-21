using Andor.Communications.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Communications.Infrastructure.Config;

public class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> entity)
    {
        _ = entity.ToTable(nameof(Message), "Communication");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => value);

        _ = entity.Property(k => k.RecipientId)
            .HasConversion(id => id.Value, value => value);

        _ = entity.Property(x => x.Title).HasMaxLength(100);
        _ = entity.Property(x => x.Body);
        _ = entity.Property(x => x.SentAt);

        _ = entity.HasIndex(x => x.RecipientId);
    }
}
