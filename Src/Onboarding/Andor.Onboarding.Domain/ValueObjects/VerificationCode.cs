namespace Andor.Onboarding.Domain.ValueObjects;

public readonly record struct VerificationCode
{
    public static VerificationCode Empty => new VerificationCode() { Value = string.Empty };

    public string Value { get; init; }

    private VerificationCode(string value)
    {
        Value = value;
    }

    public static VerificationCode New() => new VerificationCode(Random.Shared.NextInt64(0, 1_000_000).ToString("D6"));

    public static VerificationCode Load(string value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator VerificationCode(string value) => new(value);

    public static implicit operator string(VerificationCode id) => id.Value;
}
