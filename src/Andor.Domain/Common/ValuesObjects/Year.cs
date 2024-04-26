using Andor.Domain.Validation;

namespace Andor.Domain.Common.ValuesObjects;

public record struct Year
{
    public int Value { get; }

    private static void ValidateYear(int value)
    {
        var maxYear = DateTime.UtcNow.Year + 100;
        if (value < 1950 || value > maxYear)
        {
            throw new InvalidDataException(
                DefaultsErrorsMessages.BetweenLength.GetMessage(1950, maxYear));
        }
    }

    public Year(int value)
    {
        ValidateYear(value);
        Value = value;
    }

    public static Year New(int year) => new(year);

    public static Year Load(string value)
    {
        if (!int.TryParse(value, out int guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new Year(guid);
    }

    public static Year Load(int value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator Year(int value) => new(value);

    public static implicit operator int(Year id) => id.Value;
}
