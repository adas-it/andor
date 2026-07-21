using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;

namespace Andor.Accounts.Application.Commands;

public record SeedAccountDefaultsCommand(AccountId Id, ApplicationUser CurrentUser, CancellationToken CancellationToken)
    : ICommands<AccountId>;
