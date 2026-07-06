using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.PaymentMethods.ValueObjects;

public readonly record struct PaymentMethodId : IId<PaymentMethodId>
{
    public static PaymentMethodId Empty => new PaymentMethodId() { Value = Guid.Empty };

    private PaymentMethodId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; init; }
    public static PaymentMethodId New() => new(Guid.NewGuid());

    public static PaymentMethodId Load(string value)
    {
        return !Guid.TryParse(value, out var guid) ?
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value)) : new PaymentMethodId(guid);
    }

    public static PaymentMethodId Load(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator PaymentMethodId(Guid value) => new(value);

    public static implicit operator Guid(PaymentMethodId id) => id.Value;
}