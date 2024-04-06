using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Communications.ValueObjects;

public record struct RuleId(Guid Value)
{
    public static RuleId New() => new(Guid.NewGuid());

    public static RuleId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new RuleId(guid);
    }

    public static RuleId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator RuleId(Guid value) => new(value);

    public static implicit operator Guid(RuleId id) => id.Value;
}

