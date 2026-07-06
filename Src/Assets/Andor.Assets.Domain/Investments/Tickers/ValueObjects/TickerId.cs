using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Tickers.ValueObjects;

public readonly record struct TickerId : IId<TickerId>
{
    public static TickerId Empty => Guid.Empty;

    private TickerId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static TickerId New() => new(Guid.NewGuid());

    public static TickerId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new TickerId(guid);
    }

    public static TickerId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator TickerId(Guid value) => new(value);

    public static implicit operator Guid(TickerId id) => id.Value;
}
