using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Onboarding.Domain.ValueObjects;

public readonly record struct SignupRequestId : IId<SignupRequestId>
{
    public static SignupRequestId Empty => new SignupRequestId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private SignupRequestId(Guid value)
    {
        Value = value;
    }

    public static SignupRequestId New() => new SignupRequestId(Guid.NewGuid());

    public static SignupRequestId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new SignupRequestId(guid);
    }

    public static SignupRequestId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator SignupRequestId(Guid value) => new(value);

    public static implicit operator Guid(SignupRequestId id) => id.Value;
}
