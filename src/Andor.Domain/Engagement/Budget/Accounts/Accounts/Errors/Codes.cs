#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Andor.Domain.Common.ValuesObjects;
public sealed partial record DomainErrorCode
{
    public static readonly DomainErrorCode AccountErrorOnDelete = new(3_001);
}
