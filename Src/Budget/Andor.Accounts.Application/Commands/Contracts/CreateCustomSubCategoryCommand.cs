using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Commands.Contracts;

public record CreateCustomSubCategoryCommand(
    AccountId Id,
    Name Name,
    Description Description,
    CategoryId CategoryId,
    PaymentMethodId? DefaultPaymentMethodId,
    ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AccountId>;
