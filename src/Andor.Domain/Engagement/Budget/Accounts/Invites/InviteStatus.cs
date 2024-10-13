using Andor.Domain.Common;

namespace Andor.Domain.Engagement.Budget.Accounts.Invites;

public record InviteStatus : Enumeration<int>
{
    private InviteStatus(int key, string name) : base(key, name) { }

    public static readonly InviteStatus Undefined = new(0, "undefined");
    public static readonly InviteStatus Pending = new(1, "Pending");
    public static readonly InviteStatus Accepted = new(2, "Accepted");
    public static readonly InviteStatus Refused = new(2, "Refused");
}
