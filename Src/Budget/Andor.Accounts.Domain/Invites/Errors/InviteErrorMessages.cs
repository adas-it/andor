namespace Andor.Accounts.Domain.Invites.Errors;

internal static class InviteErrorMessages
{
    public const string InviteNotActive = "Cannot perform this action on an inactive invite.";
    public const string InviteAlreadyAccepted = "Invite has already been accepted.";
    public const string CannotRejectAcceptedInvite = "Cannot reject an accepted invite.";
    public const string InviteAlreadyLinkedToUser = "Invite is already linked to a user.";
    public const string InviteNotLinkedToUser = "Cannot accept invite without a linked user. User must be created first.";
    public const string InviteMustHaveEmailOrUserId = "Invite must have either an email or a user ID.";
}
