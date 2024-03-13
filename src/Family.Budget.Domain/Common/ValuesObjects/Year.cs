namespace Family.Budget.Domain.Common.ValuesObjects;

using Family.Budget.Domain.Exceptions;
using System;

public record Year
{
    public int Value { get; }

    public Year(int value)
    {
        ValidateYear(value);
        Value = value;
    }

    private static void ValidateYear(int value)
    {
        var maxYear = DateTime.UtcNow.Year + 100;
        if (value < 2000 || value > maxYear)
        {
            throw new InvalidDomainException(
                DefaultsErrorsMessages.BetweenLength.GetMessage(2000, maxYear),
                CommonErrorCodes.InvalidYear);
        }
    }

    public static Year New(int year) => new(year);

    public override string ToString() => Value.ToString();

    public static implicit operator Year(int value) => new(value);

    public static implicit operator int(Year year) => year.Value;
}