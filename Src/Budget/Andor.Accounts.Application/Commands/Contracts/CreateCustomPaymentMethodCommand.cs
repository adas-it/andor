using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Commands.Contracts;

public record CreateCustomPaymentMethodCommand(
    AccountId Id,
    Name Name,
    Description Description,
    MovementType Type,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
