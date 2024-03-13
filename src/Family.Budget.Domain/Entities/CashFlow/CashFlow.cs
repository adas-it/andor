namespace Family.Budget.Domain.Entities.CashFlow;

using Family.Budget.Domain.Entities.CashFlow.DomainEvents;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.SeedWork;

public class CashFlow : Entity
{
    private CashFlow()
    {

    }
    private CashFlow(Guid id, int year, int month, Guid accountId, decimal netRevenue, decimal finalBalancePreviousMonth, decimal payments, decimal accountBalance, decimal forecastNextPayments, decimal forecastUpcomingRevenues)
    {
        Year = year;
        Month = month;
        AccountId = accountId;
        MonthRevenues = netRevenue;
        FinalBalancePreviousMonth = finalBalancePreviousMonth;
        Expenses = payments;
        AccountBalance = accountBalance;
        ForecastExpenses = forecastNextPayments;
        ForecastUpcomingRevenues = forecastUpcomingRevenues;
        Id = id;
    }

    public int Year { get; private set; }
    public int Month { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal FinalBalancePreviousMonth { get; private set; }
    public decimal MonthRevenues { get; private set; }
    public decimal ForecastUpcomingRevenues { get; private set; }
    public decimal RevenuesBalance
    {
        get
        {
            return MonthRevenues + FinalBalancePreviousMonth;
        }
    }
    public decimal Expenses { get; private set; }
    public decimal ForecastExpenses { get; private set; }
    public decimal AccountBalance { get; private set; }
    public decimal BalanceForecast
    {
        get
        {
            return ((RevenuesBalance + ForecastUpcomingRevenues) - (Expenses + ForecastExpenses));
        }
    }
    public decimal MonthlyDeficitSurplus
    {
        get
        {
            return (MonthRevenues + ForecastUpcomingRevenues) - (Expenses + ForecastExpenses);
        }
    }

    public static CashFlow New(int Year, int Month, Guid AccountId, decimal finalBalancePreviousMonth)
    {
        var entity = new CashFlow(Guid.NewGuid(), Year, Month, AccountId, 0, finalBalancePreviousMonth, 0, 0, 0, 0);

        entity.RaiseDomainEvent(new CashFlowEventsCreatedDomainEvent(entity));

        return entity;
    }

    private void AddRevenue(decimal value)
    {
        MonthRevenues += value;

        SetAccountBalance();

        Validate();
    }

    private void RemoveRevenue(decimal value)
    {
        MonthRevenues -= value;

        SetAccountBalance();

        Validate();
    }

    private void AddForecastRevenue(decimal value)
    {
        ForecastUpcomingRevenues += value;

        Validate();
    }

    private void RemoveForecastRevenue(decimal value)
    {
        ForecastUpcomingRevenues -= value;

        Validate();
    }

    private void AddExpense(decimal value)
    {
        Expenses += value;

        SetAccountBalance();

        Validate();
    }

    private void RemoveExpense(decimal value)
    {
        Expenses -= value;

        SetAccountBalance();

        Validate();
    }

    private void AddForecastExpense(decimal value)
    {
        ForecastExpenses += value;

        Validate();
    }

    private void RemoveForecastExpense(decimal value)
    {
        ForecastExpenses -= value;

        Validate();
    }

    public void SetFinalBalancePreviousMonth(decimal value)
    {
        FinalBalancePreviousMonth = value;

        SetAccountBalance();

        Validate();
    }

    private void SetAccountBalance()
    {
        AccountBalance = RevenuesBalance - Expenses;

        RaiseDomainEvent(new AccountBalanceChangedDomainEvent(this));
    }

    public void AddFinancialMovement(FinancialMovement financialMovement)
    {
        if (financialMovement.Type == MovementType.MoneyDeposit)
        {
            if (financialMovement.Status == MovementStatus.Accomplished)
            {
                AddRevenue(financialMovement.Value);
            }

            if (financialMovement.Status == MovementStatus.Expected)
            {
                AddForecastRevenue(financialMovement.Value);
            }
        }

        if (financialMovement.Type == MovementType.MoneySpending)
        {
            if (financialMovement.Status == MovementStatus.Accomplished)
            {
                AddExpense(financialMovement.Value);
            }

            if (financialMovement.Status == MovementStatus.Expected)
            {
                AddForecastExpense(financialMovement.Value);
            }
        }
    }

    public void UpdateFinancialMovement(FinancialMovement oldMovement, FinancialMovement movement)
    {
        if (oldMovement.Type == MovementType.MoneySpending)
        {
            if (oldMovement.Status == MovementStatus.Expected)
            {
                RemoveForecastExpense(oldMovement.Value);
            }

            if (oldMovement.Status == MovementStatus.Accomplished)
            {
                RemoveExpense(oldMovement.Value);
            }

            if (movement.Status == MovementStatus.Expected)
            {
                AddForecastExpense(movement.Value);
            }

            if (movement.Status == MovementStatus.Accomplished)
            {
                AddExpense(movement.Value);
            }

            return;
        }

        if (oldMovement.Type == MovementType.MoneyDeposit)
        {
            if (oldMovement.Status == MovementStatus.Expected)
            {
                RemoveForecastRevenue(oldMovement.Value);
            }

            if (oldMovement.Status == MovementStatus.Accomplished)
            {
                RemoveRevenue(oldMovement.Value);
            }

            if (movement.Status == MovementStatus.Expected)
            {
                AddForecastRevenue(movement.Value);
            }

            if (movement.Status == MovementStatus.Accomplished)
            {
                AddRevenue(movement.Value);
            }

            return;
        }
    }

    public void RemovedFinancialMovement(FinancialMovement movement)
    {
        if (movement.Type == MovementType.MoneySpending)
        {
            if (movement.Status == MovementStatus.Expected)
            {
                RemoveForecastExpense(movement.Value);
            }

            if (movement.Status == MovementStatus.Accomplished)
            {
                RemoveExpense(movement.Value);
            }

            return;
        }

        if (movement.Type == MovementType.MoneyDeposit)
        {
            if (movement.Status == MovementStatus.Expected)
            {
                RemoveForecastRevenue(movement.Value);
            }

            if (movement.Status == MovementStatus.Accomplished)
            {
                RemoveRevenue(movement.Value);
            }

            return;
        }
    }
}
