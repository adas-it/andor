using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.Invites.Errors;

public sealed record InviteErrorCode
{
    public static readonly DomainErrorCode InviteNotActive = DomainErrorCode.New(4_001);
    public static readonly DomainErrorCode InviteAlreadyAccepted = DomainErrorCode.New(4_002);
    public static readonly DomainErrorCode CannotRejectAcceptedInvite = DomainErrorCode.New(4_003);
    public static readonly DomainErrorCode InviteAlreadyLinkedToUser = DomainErrorCode.New(4_004);
    public static readonly DomainErrorCode InviteNotLinkedToUser = DomainErrorCode.New(4_005);
    public static readonly DomainErrorCode InviteMustHaveEmailOrUserId = DomainErrorCode.New(4_006);
}
