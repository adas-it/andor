using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Currencies.ValueObjects;

public record struct Iso
{
    public static readonly int MinLength = 3;
    public static readonly int MaxLength = 3;

    public string Value { get; init; }

    private Iso(string value)
    {
        Value = value;

        var ret = Value.BetweenLength(MinLength, MaxLength);

        if (ret != null)
        {
            throw new ArgumentException(ret.Message);
        }
    }

    public static Iso Load(string value)
    {
        return new Iso() with { Value = value };
    }

    public override readonly string ToString() => Value.ToString();

    public static implicit operator Iso(string value) => new Iso() with { Value = value };

    public static implicit operator string(Iso id) => id.Value;
}