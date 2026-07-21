using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.CashFlows;

/// <summary>
/// Read-model projection of an account's monthly balance. Maintained exclusively by the
/// CashFlow projection consumer reacting to financial movement events — not an aggregate
/// root, raises no domain events, and is never written to directly by the Account aggregate.
/// </summary>
public class CashFlow : Entity<CashFlowId>
{
    public AccountId AccountId { get; private set; }
    public Year Year { get; private set; }
    public Month Month { get; private set; }

    /// <summary>
    /// Persisted as Year*100+Month purely to make ordering/uniqueness queries simple.
    /// </summary>
    public int PeriodKey { get; private set; }

    public decimal FinalBalancePreviousMonth { get; private set; }
    public decimal MonthRevenues { get; private set; }
    public decimal ForecastUpcomingRevenues { get; private set; }
    public decimal Expenses { get; private set; }
    public decimal ForecastExpenses { get; private set; }
    public decimal AccountBalance { get; private set; }

    public decimal RevenuesBalance => MonthRevenues + FinalBalancePreviousMonth;

    public decimal BalanceForecast =>
        (RevenuesBalance + ForecastUpcomingRevenues) - (Expenses + ForecastExpenses);

    public decimal MonthlyDeficitSurplus =>
        (MonthRevenues + ForecastUpcomingRevenues) - (Expenses + ForecastExpenses);

    protected CashFlow()
    {
    }

    private CashFlow(CashFlowId id, AccountId accountId, Year year, Month month, decimal finalBalancePreviousMonth)
    {
        Id = id;
        AccountId = accountId;
        Year = year;
        Month = month;
        PeriodKey = (year.Value * 100) + month.Value;
        FinalBalancePreviousMonth = finalBalancePreviousMonth;
        AccountBalance = finalBalancePreviousMonth;
    }

    public static (DomainResult, CashFlow?) New(
        CashFlowId id,
        AccountId accountId,
        Year year,
        Month month,
        decimal finalBalancePreviousMonth)
    {
        var entity = new CashFlow(id, accountId, year, month, finalBalancePreviousMonth);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    public static (DomainResult, CashFlow?) New(
        AccountId accountId,
        Year year,
        Month month,
        decimal finalBalancePreviousMonth)
        => New(CashFlowId.New(), accountId, year, month, finalBalancePreviousMonth);

    /// <summary>
    /// Applies a financial movement's value to the corresponding revenue/expense bucket
    /// (based on its type and status) and recomputes the month's account balance.
    /// </summary>
    public void ApplyMovement(MovementType type, MovementStatus status, decimal value)
    {
        if (type == MovementType.MoneyDeposit)
        {
            if (status == MovementStatus.Accomplished)
                MonthRevenues += value;

            if (status == MovementStatus.Expected)
                ForecastUpcomingRevenues += value;
        }

        if (type == MovementType.MoneySpending)
        {
            if (status == MovementStatus.Accomplished)
                Expenses += value;

            if (status == MovementStatus.Expected)
                ForecastExpenses += value;
        }

        RecomputeAccountBalance();
    }

    /// <summary>
    /// Re-anchors this month's opening balance to the previous month's closing balance.
    /// Used when cascading a change forward across already-existing later months.
    /// </summary>
    public void SetFinalBalancePreviousMonth(decimal value)
    {
        FinalBalancePreviousMonth = value;
        RecomputeAccountBalance();
    }

    private void RecomputeAccountBalance()
    {
        AccountBalance = RevenuesBalance - Expenses;
    }

    protected override DomainResult Validate()
    {
        return base.Validate();
    }
}
