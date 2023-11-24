namespace Family.Budget.Infrastructure.Repositories.Notifications;
using Family.Budget.Domain.Entities.Notifications;
using Family.Budget.Domain.Entities.Notifications.NotificationTypes;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        var typeConverter = new ValueConverter<NotificationType, int>(
        v => v.Key,
        v => NotificationType.GetByKey<NotificationType>(v));

        entity.ToTable(nameof(Notification), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Property(k => k.Type).HasConversion(typeConverter);
        entity.Ignore(k => k.Events);
    }
}