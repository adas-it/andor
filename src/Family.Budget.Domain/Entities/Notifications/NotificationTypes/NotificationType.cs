namespace Family.Budget.Domain.Entities.Notifications.NotificationTypes;
using Family.Budget.Domain.Common;

public record NotificationType : Enumeration<int>
{
    private NotificationType(int key, string name) : base(key, name)
    {
    }

    public static readonly NotificationType Movement = new(1, "movement");
    public static readonly NotificationType InviteAccount = new(2, "invite-account");
}