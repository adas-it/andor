namespace Family.Budget.Domain.Common.ValuesObjects;
using Family.Budget.Domain.Exceptions;

public record Month
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
            throw new InvalidDomainException(
                DefaultsErrorsMessages.BetweenLength.GetMessage(1, 12),
                CommonErrorCodes.InvalidMonth);
        }
    }

    public static Month New(int month) => new(month);

    public override string ToString() => Value.ToString();

    public static implicit operator Month(int value) => new(value);

    public static implicit operator int(Month month) => month.Value;
}
