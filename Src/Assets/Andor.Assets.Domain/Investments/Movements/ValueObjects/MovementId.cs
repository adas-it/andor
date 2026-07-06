using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Movements.ValueObjects;

public readonly record struct MovementId : IId<MovementId>
{
    public static MovementId Empty => Guid.Empty;

    private MovementId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static MovementId New() => new(Guid.NewGuid());

    public static MovementId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new MovementId(guid);
    }

    public static MovementId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator MovementId(Guid value) => new(value);

    public static implicit operator Guid(MovementId id) => id.Value;
}
