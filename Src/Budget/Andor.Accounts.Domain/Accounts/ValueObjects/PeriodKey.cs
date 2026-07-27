using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts.ValueObjects;

public struct PeriodKey
{
    public static PeriodKey Empty => new PeriodKey() { Value = 0 };
    
    public int Value { get; init; }
    
    public static PeriodKey Load(int value)
    {
        return new PeriodKey() { Value = value };
    }
    
    public static PeriodKey From(Year year, Month month )
    {
        return new PeriodKey(year, month);
    }

    private PeriodKey(Year year, Month month)
    {
        Value = (year.Value * 100) + month.Value;
    }
    
    public override string ToString() => Value.ToString();

    public static implicit operator PeriodKey(int value) => new PeriodKey() { Value = value };

    public static implicit operator int(PeriodKey periodKey) => periodKey.Value;
}