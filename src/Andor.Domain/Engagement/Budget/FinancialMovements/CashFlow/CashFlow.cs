using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;

public class CashFlow : Entity<CashFlowId>
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

    private DomainResult SetValues(CashFlowId id,
        string name,
        string description,
        Currency? currency,
        bool deleted,
        DateTime? firstMovement,
        DateTime? lastMovement)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 70));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;

        var result = Validate();

        return result;
    }
}
