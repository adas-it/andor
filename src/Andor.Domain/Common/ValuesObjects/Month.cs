using Andor.Domain.Validation;

namespace Andor.Domain.Common.ValuesObjects;

public record struct Month
{
    public int Value { get; }

    public Month(int value)
    {
        ValidateMonth(value);
        Value = value;
    }

    private static void ValidateMonth(int value)
    {
        if (value < 1 || value > 12)
        {
            throw new InvalidDataException(
                DefaultsErrorsMessages.BetweenLength.GetMessage(1, 12));
        }
    }

    public static Month New(int month) => new(month);

    public static Month Load(string value)
    {
        if (!int.TryParse(value, out int guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new Month(guid);
    }

    public static Month Load(int value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator Month(int value) => new(value);

    public static implicit operator int(Month id) => id.Value;
}
