using Andor.Domain.Common;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;

public record MovementStatus : Enumeration<int>
{
    private MovementStatus(int key, string name) : base(key, name)
    {
    }

    public static readonly MovementStatus Accomplished = new(1, "accomplished");
    public static readonly MovementStatus Expected = new(2, "expected");
}
