namespace Andor.Foundation.Domain.ValuesObjects;

public record Description : StringValueObject
{
    public const int MinLength = 3;
    public const int MaxLength = 250;

    public Description(string value) : base(value, MinLength, MaxLength, nameof(Description)) { }

    public static implicit operator Description(string value) => new(value);

    public static implicit operator string?(Description? symbol) => symbol?.Value;
    public virtual bool Equals(Name? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value,
            StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
        => Value?.ToUpperInvariant().GetHashCode() ?? 0;
}
