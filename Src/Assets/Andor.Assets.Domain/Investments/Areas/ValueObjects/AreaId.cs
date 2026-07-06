using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Areas.ValueObjects;

public readonly record struct AreaId : IId<AreaId>
{
    public static AreaId Empty => new(Guid.Empty);

    private AreaId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static AreaId New() => new(Guid.NewGuid());

    public static AreaId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new AreaId(guid);
    }

    public static AreaId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator AreaId(Guid value) => new(value);

    public static implicit operator Guid(AreaId id) => id.Value;
}
