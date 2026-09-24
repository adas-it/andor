using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Commands.Contracts;

public record UpdateAccountDetailsCommand(AccountId Id, Name Name, Description Description, CurrencyId CurrencyId,
    ApplicationUser CurrentUser, CancellationToken CancellationToken) : ICommands<AccountId>;
