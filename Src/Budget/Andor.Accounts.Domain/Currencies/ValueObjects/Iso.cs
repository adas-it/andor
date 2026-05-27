namespace Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;

public record struct Iso
{
    public static readonly int MinLength = 3;
    public static readonly int MaxLength = 3;

    public string Value { get; init; }

    public static Iso Load(string value)
    {
        return new Iso() with { Value = value };
    }

    public override readonly string ToString() => Value.ToString();

    public static implicit operator Iso(string value) => new Iso() with { Value = value };

    public static implicit operator string(Iso id) => id.Value;
}
