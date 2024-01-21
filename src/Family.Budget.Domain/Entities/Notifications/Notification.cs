namespace Family.Budget.Domain.Entities.Notifications;

using Family.Budget.Domain.Entities.Notifications.NotificationTypes;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;

public class Notification : Entity
{
    public Guid EntityId { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public NotificationType Type { get; private set; }
    public Guid UserId { get; private set; }

    private Notification()
    { }

    private Notification(Guid id, Guid entityId, string description, bool opened, NotificationType type, Guid userId)
    {
        Id  = id;
        EntityId = entityId;
        Description = description;
        Opened = opened;
        Type = type;
        UserId = userId;
    }

    public static Notification New(Guid entityId,
        string description,
        bool opened,
        NotificationType type, Guid userId)
    {
        var entity = new Notification(Guid.NewGuid(),
            entityId,
            description,
            opened,
            type,
            userId);

        //entity.RaiseDomainEvent(new PaymentMethodCreatedDomainEvent(entity));

        return entity;
    }

    protected override void Validate()
    {
        AddNotification(Id.NotNull());
        AddNotification(EntityId.NotNull());
        AddNotification(UserId.NotNull());

        AddNotification(Description.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Description.BetweenLength(3, 1000));

        base.Validate();
    }
}
