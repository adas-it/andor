using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.DomainEvents;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;

public class CashFlow : AggregateRoot<CashFlowId>
{
    public Year Year { get; private set; }
    public Month Month { get; private set; }
    public AccountId AccountId { get; private set; }
    public Account Account { get; private set; }
    public decimal FinalBalancePreviousMonth { get; private set; }
    public decimal MonthRevenues { get; private set; }
    public decimal ForecastUpcomingRevenues { get; private set; }
    public decimal Expenses { get; private set; }
    public decimal ForecastExpenses { get; private set; }
    public decimal AccountBalance { get; private set; }
    public decimal RevenuesBalance
    {
        get
        {
            return MonthRevenues + FinalBalancePreviousMonth;
        }
    }
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

    private CashFlow()
    {
    }

    public static (DomainResult, CashFlow) New(Year year,
        Month month,
        Account account,
        decimal finalBalancePreviousMonth)
    {
        var entity = new CashFlow();

        var result = entity.SetValues(CashFlowId.New(),
            year,
            month,
            account,
            finalBalancePreviousMonth,
            0,
            0,
            0,
            0,
            0);

        return (result, entity);
    }

    private DomainResult SetValues(CashFlowId id,
        Year year,
        Month month,
        Account account,
        decimal finalBalancePreviousMonth,
        decimal monthRevenues,
        decimal forecastUpcomingRevenues,
        decimal expenses,
        decimal forecastExpenses,
        decimal accountBalance)
    {
        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Year = year;
        Month = month;
        AccountId = account.Id;
        Account = account;
        FinalBalancePreviousMonth = finalBalancePreviousMonth;
        MonthRevenues = monthRevenues;
        ForecastUpcomingRevenues = forecastUpcomingRevenues;
        Expenses = expenses;
        ForecastExpenses = forecastExpenses;
        AccountBalance = accountBalance;

        var result = Validate();

        return result;
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

        RaiseDomainEvent(new AccountBalanceChangedDomainEvent()
        {
            Current = CashFlowDomainEvent.FromAggregator(this)
        });
    }

    public void AddFinancialMovement(MovementType type, MovementStatus status, decimal value)
    {
        if (type == MovementType.MoneyDeposit)
        {
            if (status == MovementStatus.Accomplished)
            {
                AddRevenue(value);
            }

            if (status == MovementStatus.Expected)
            {
                AddForecastRevenue(value);
            }
        }

        if (type == MovementType.MoneySpending)
        {
            if (status == MovementStatus.Accomplished)
            {
                AddExpense(value);
            }

            if (status == MovementStatus.Expected)
            {
                AddForecastExpense(value);
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
