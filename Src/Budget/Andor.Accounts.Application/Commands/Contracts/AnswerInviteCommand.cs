using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;

namespace Andor.Accounts.Application.Commands.Contracts;

public record AnswerInviteCommand(
    AccountId Id,
    InviteId InviteId,
    bool Accept,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
