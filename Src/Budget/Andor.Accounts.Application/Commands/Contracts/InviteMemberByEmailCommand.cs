using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Commands.Contracts;

public record InviteMemberByEmailCommand(
    AccountId Id,
    Email Email,
    PermissionType Permission,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
