using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Users.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;

namespace Andor.Accounts.Application.Commands.Contracts;

public record InviteMemberByUserCommand(
    AccountId Id,
    UserId InvitedUserId,
    PermissionType Permission,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
