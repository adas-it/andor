using Andor.Domain.Validation;

namespace Andor.Domain.Onboarding.Registrations.ValueObjects;

public record struct CheckCode
{
    public static readonly int MinLength = 4;
    public static readonly int MaxLength = 4;

    public string Value { get; init; }

    private CheckCode(string value)
    {
        Value = value;

        var ret = Value.BetweenLength(MinLength, MaxLength);

        if (ret != null)
        {
            throw new ArgumentException(ret.Message);
        }
    }

    public static CheckCode New() => new(GetNewCheckCode());
    public static CheckCode Load(string value)
    {
        return new CheckCode(value);
    }

    public override readonly string ToString() => Value.ToString();

    public static implicit operator CheckCode(string value) => new(value);

    public static implicit operator string(CheckCode id) => id.Value;

    private static string GetNewCheckCode()
    {
        var random = new Random();

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
