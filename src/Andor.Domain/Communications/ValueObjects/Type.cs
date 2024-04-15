using Andor.Domain.Common;

namespace Andor.Domain.Communications.ValueObjects;

public record Type : Enumeration<int>
{
    private Type(int id, string name) : base(id, name)
    {
    }

    public static readonly Type Undefined = new(0, nameof(Undefined));
    public static readonly Type Information = new(1, nameof(Information));
    public static readonly Type Marketing = new(2, nameof(Marketing));
}
