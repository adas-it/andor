using Andor.Domain.Common;

namespace Andor.Domain.Engagement.Budget.Entities.MovementTypes;

public record MovementType : Enumeration<int>
{
    private MovementType(int key, string name) : base(key, name)
    {
    }

    public static readonly MovementType Undefined = new(0, "undefined");
    public static readonly MovementType MoneyDeposit = new(1, "money-deposit");
    public static readonly MovementType MoneySpending = new(2, "money-spending");
}
