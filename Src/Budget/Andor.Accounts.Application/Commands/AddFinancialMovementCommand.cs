using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;

namespace Andor.Accounts.Application.Commands;

public record AddFinancialMovementCommand(
    AccountId Id,
    DateTime Date,
    string? Description,
    SubCategoryId SubCategoryId,
    PaymentMethodId PaymentMethodId,
    decimal Value,
    MovementStatus? Status,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
