namespace Andor.Foundation.Domain.ValuesObjects;

public record Value : StringValueObject
{
    public static Value Empty => new Value();

    public const int MinLength = 1;
    public const int MaxLength = 2500;

    private Value() : base() { }

    public Value(string value) : base(value, MinLength, MaxLength, nameof(Value)) { }

    public static implicit operator Value(string value) => new(value);

    public static implicit operator string?(Value? symbol) => symbol?.Value;
    public virtual bool Equals(Value? other)
    {
        if (other is null)
            return false;
        return string.Equals(Value, other.Value,
            StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
        => Value?.ToUpperInvariant().GetHashCode() ?? 0;
}
