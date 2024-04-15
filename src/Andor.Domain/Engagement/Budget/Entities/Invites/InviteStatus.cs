using Andor.Domain.Common;

namespace Andor.Domain.Engagement.Budget.Entities.Invites;

public record InviteStatus : Enumeration<int>
{
    private InviteStatus(int key, string name) : base(key, name) { }

    public static readonly InviteStatus Undefined = new(0, "undefined");
    public static readonly InviteStatus Draft = new(1, "Draft");
    public static readonly InviteStatus Sent = new(2, "Sent");
    public static readonly InviteStatus Accepted = new(2, "Accepted");
}
