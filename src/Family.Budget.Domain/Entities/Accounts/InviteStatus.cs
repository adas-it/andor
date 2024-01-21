namespace Family.Budget.Domain.Entities.Accounts;

using Family.Budget.Domain.Common;

public record InviteStatus : Enumeration<int>
{
    private InviteStatus(int key, string name) : base(key, name){ }

    public static readonly InviteStatus Draft = new(1, "Draft");
    public static readonly InviteStatus Sended = new(2, "Sended");
    public static readonly InviteStatus Accepted = new(2, "Accepted");
}
