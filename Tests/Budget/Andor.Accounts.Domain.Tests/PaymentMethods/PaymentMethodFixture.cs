using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;

namespace Andor.Accounts.Domain.Tests.PaymentMethods;

internal static class PaymentMethodFixture
{
    public static PaymentMethod GetTemplatePaymentMethod(
        PaymentMethodId? id = null,
        Name? name = null,
        Description? description = null,
        MovementType? type = null)
        => CreatePaymentMethod(id: id, name: name, description: description, type: type);

    public static PaymentMethod GetCustomPaymentMethodWithOwner(
        PaymentMethodId? id = null,
        Name? name = null,
        Description? description = null,
        MovementType? type = null,
        AccountId? owner = null)
        => CreatePaymentMethod(id: id, name: name, description: description, type: type, owner: owner);

    private static PaymentMethod CreatePaymentMethod(
        PaymentMethodId? id = null,
        Name? name = null,
        Description? description = null,
        MovementType? type = null,
        AccountId? owner = null)
    {
        id ??= PaymentMethodId.New();
        name ??= GeneralFixture.GetValidName();
        description ??= GeneralFixture.GetValidDescription();
        type ??= MovementType.MoneySpending;

        var (_, result) = PaymentMethod.New(
            id: (PaymentMethodId)id,
            name: name,
            description: description,
            type: type,
            owner: owner);

        return result!;
    }
}
