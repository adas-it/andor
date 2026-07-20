using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.ValueObjects;

public readonly record struct RuleId : IId<RuleId>
{
    public static RuleId Empty => new RuleId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private RuleId(Guid value)
    {
        Value = value;
    }

    public static RuleId New() => new RuleId(Guid.NewGuid());

    public static RuleId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new RuleId(guid);
    }

    public static RuleId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator RuleId(Guid value) => new(value);

    public static implicit operator Guid(RuleId id) => id.Value;
}

