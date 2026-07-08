namespace Andor.Foundation.Domain.ValuesObjects;

public abstract record StringValueObject
{
    public string? Value { get; }

    protected StringValueObject()
    {
        Value = string.Empty;
    }

    protected StringValueObject(string? value, int minLength, int maxLength, string name)
    {
        if (value is null)
        {
            return;
        }

        value = value?.Trim() ?? string.Empty;

        if (value.Length < minLength || value.Length > maxLength)
            throw new ArgumentOutOfRangeException(nameof(value),
                $"{name} must be between {minLength} and {maxLength} characters.");

        Value = value;
    }

    public sealed override string ToString() => Value ?? string.Empty;
    public bool HasValue => Value is not null;
    public string? GetValueOrNull() => Value;
}
