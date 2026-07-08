namespace Andor.Domain.Common.ValuesObjects;

public partial record DomainErrorCode
{
    internal int Value { get; set; }
    protected DomainErrorCode(int value)
    {
        Value = value;
    }
    public override string ToString() => Value.ToString();

    public static implicit operator int(DomainErrorCode id) => id.Value;
}
