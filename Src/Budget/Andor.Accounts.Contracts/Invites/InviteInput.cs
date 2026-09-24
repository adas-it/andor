namespace Andor.Accounts.Contracts.Invites;

// Exactly one of Email/UserId must be set: Email invites someone with no system account yet,
// UserId invites someone who already exists. Validated by the controller, not here.
public record InviteInput(string? Email, Guid? UserId, int PermissionKey);
