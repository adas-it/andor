namespace Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;

using Family.Budget.Domain.Common;

public record MovementType : Enumeration<int>
{
    private MovementType(int key, string name) : base(key, name)
    {
    }

    public static readonly MovementType MoneyDeposit = new(1, "money-deposit");
    public static readonly MovementType MoneySpending = new(2, "money-spending");
}