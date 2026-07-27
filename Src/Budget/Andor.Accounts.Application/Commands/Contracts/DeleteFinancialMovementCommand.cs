using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;

namespace Andor.Accounts.Application.Commands.Contracts;

public record DeleteFinancialMovementCommand(
    AccountId Id,
    FinancialMovementId FinancialMovementId,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
