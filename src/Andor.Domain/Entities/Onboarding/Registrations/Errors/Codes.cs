namespace Andor.Domain.Common.ValuesObjects;

public sealed partial record DomainErrorCode
{
    public static readonly DomainErrorCode ErrorOnDelete = new(2_001);
}