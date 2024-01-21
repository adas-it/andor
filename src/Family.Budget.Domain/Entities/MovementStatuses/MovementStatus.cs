namespace Family.Budget.Domain.Entities.MovementStatuses;

using Family.Budget.Domain.Common;

public record MovementStatus : Enumeration<int>
{
    private MovementStatus(int key, string name) : base(key, name)
    {
    }

    public static readonly MovementStatus Accomplished = new(1, "accomplished");
    public static readonly MovementStatus Expected = new(2, "expected");
}
