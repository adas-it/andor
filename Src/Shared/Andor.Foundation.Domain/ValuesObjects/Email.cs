
using System.Text.RegularExpressions;

namespace Andor.Foundation.Domain.ValuesObjects;

public partial record Email
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public const int MaxLength = 255;

    [GeneratedRegex(EmailPattern, RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex EmailRegex();

    public static Email Empty { get; } = new Email();

    private Email()
    {
        Value = string.Empty;
    }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));
        }

        if (!EmailRegex().IsMatch(value))
        {
            throw new ArgumentException("Invalid email format.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}

