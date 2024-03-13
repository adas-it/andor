namespace Family.Budget.Application.Dto.Notifications.Responses;

using Family.Budget.Application.Dto.Notifications.NotificationType;
using System;

public class NotificationOutput
{
    public Guid EntityId { get; set; }
    public string Description { get; set; }
    public bool Opened { get; set; }
    public NotificationTypeOutput Type { get; set; }
    public Guid UserId { get; set; }
}
