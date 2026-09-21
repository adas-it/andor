namespace Andor.Users.Application.Messages;

/// <summary>
/// Published to the "request-identity-user" queue after a User is provisioned, so
/// <c>Andor.Users.WebApi</c> can create its Identity/credentials row. Per ADR-0006, the receiver
/// declares its own local type for this rather than referencing this one directly.
/// </summary>
public sealed record IdentityProvisioningRequested(Guid UserId, string Email, string Name, string PasswordHash);

/// <summary>
/// Published to the "request-account-creation" queue after a User is provisioned, so
/// <c>Andor.Accounts.Service</c> can create the user's default Account. Per ADR-0006, the
/// receiver declares its own local type for this rather than referencing this one directly.
/// </summary>
public sealed record AccountProvisioningRequested(Guid UserId, string Email, string Name);
