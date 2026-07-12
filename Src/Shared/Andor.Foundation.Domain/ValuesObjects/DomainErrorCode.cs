namespace Andor.Domain.Common.ValuesObjects;

public record DomainErrorCode
{
    internal int Value { get; set; }
    private DomainErrorCode(int value)
    {
        Value = value;
    }

    public static DomainErrorCode New(int value)
    {
        return new DomainErrorCode(value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(DomainErrorCode id) => id.Value;
}
