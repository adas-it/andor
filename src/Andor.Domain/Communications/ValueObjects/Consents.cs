using Andor.Domain.Common;

namespace Andor.Domain.Communications.ValueObjects;

public record Consents : Enumeration<int>
{
    private Consents(int id, string name) : base(id, name)
    {
    }

    public static readonly Consents Undefined = new(0, nameof(Undefined));
    public static readonly Consents Accepted = new(1, nameof(Accepted));
    public static readonly Consents Denied = new(2, nameof(Denied));
}
