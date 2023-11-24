namespace Family.Budget.Domain.Common.ValuesObjects;
public abstract record DomainErrorCode
{
    protected DomainErrorCode(int value) { Value = value; }

    public int Value { get; init; }

    public override string ToString() => Value.ToString();
}
