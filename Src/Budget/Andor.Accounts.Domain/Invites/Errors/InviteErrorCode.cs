using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.Invites.Errors;

public sealed record InviteErrorCode(int original) : DomainErrorCode(original)
{
    public static readonly DomainErrorCode InviteNotActive = new InviteErrorCode(4_001);
    public static readonly DomainErrorCode InviteAlreadyAccepted = new InviteErrorCode(4_002);
    public static readonly DomainErrorCode CannotRejectAcceptedInvite = new InviteErrorCode(4_003);
    public static readonly DomainErrorCode InviteAlreadyLinkedToUser = new InviteErrorCode(4_004);
    public static readonly DomainErrorCode InviteNotLinkedToUser = new InviteErrorCode(4_005);
    public static readonly DomainErrorCode InviteMustHaveEmailOrUserId = new InviteErrorCode(4_006);
}
