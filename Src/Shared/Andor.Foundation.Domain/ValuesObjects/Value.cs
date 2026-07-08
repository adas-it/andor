namespace Andor.Foundation.Domain.ValuesObjects;

public record Value : StringValueObject
{
    public const int MinLength = 1;
    public const int MaxLength = 2500;

    private Value(string value) : base(value, MinLength, MaxLength, nameof(Value)) { }

    public static implicit operator Value(string value) => new(value);

    public static implicit operator string?(Value? symbol) => symbol?.Value;
}
