namespace Andor.Foundation.Domain.ValuesObjects;

public record Name : StringValueObject
{
    public const int MinLength = 3;
    public const int MaxLength = 70;

    public Name(string value) : base(value, MinLength, MaxLength, nameof(Name)) { }

    public static implicit operator Name(string value) => new(value);

    public static implicit operator string?(Name? symbol) => symbol?.Value;

    public virtual bool Equals(Name? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value,
            StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
        => Value?.ToUpperInvariant().GetHashCode() ?? 0;
}
