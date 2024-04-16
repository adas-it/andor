using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;

public record struct PaymentMethodId
{
    private PaymentMethodId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static PaymentMethodId New() => new(Guid.NewGuid());

    public static PaymentMethodId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new PaymentMethodId(guid);
    }

    public static PaymentMethodId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator PaymentMethodId(Guid value) => new(value);

    public static implicit operator Guid(PaymentMethodId id) => id.Value;
}