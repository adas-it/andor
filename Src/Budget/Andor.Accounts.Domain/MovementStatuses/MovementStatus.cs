using Andor.Foundation.Domain;

namespace Andor.Accounts.Domain.MovementStatuses;

public sealed record MovementStatus : Enumeration<int>
{
    private MovementStatus(int key, string name) : base(key, name)
    {
    }

    public static readonly MovementStatus Undefined = new(0, "undefined");
    public static readonly MovementStatus Accomplished = new(1, "accomplished");
    public static readonly MovementStatus Expected = new(2, "expected");
}
