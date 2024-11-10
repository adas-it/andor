namespace Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.DomainEvents;

public record CashFlowDomainEvent
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal AccountBalance { get; set; }
    public Guid AccountId { get; set; }

    public decimal FinalBalancePreviousMonth { get; set; }
    public decimal MonthRevenues { get; set; }
    public decimal ForecastUpcomingRevenues { get; set; }
    public decimal Expenses { get; set; }
    public decimal ForecastExpenses { get; set; }
    public decimal RevenuesBalance { get; set; }
    public decimal BalanceForecast { get; set; }
    public decimal MonthlyDeficitSurplus { get; set; }

    public static CashFlowDomainEvent FromAggregator(CashFlow entity)
        => new CashFlowDomainEvent() with
        {
            Id = entity.Id,
            Year = entity.Year,
            Month = entity.Month,
            AccountId = entity.AccountId,
            AccountBalance = entity.AccountBalance,
            FinalBalancePreviousMonth = entity.FinalBalancePreviousMonth,
            MonthRevenues = entity.MonthRevenues,
            ForecastUpcomingRevenues = entity.ForecastUpcomingRevenues,
            Expenses = entity.Expenses,
            ForecastExpenses = entity.ForecastExpenses,
            RevenuesBalance = entity.RevenuesBalance,
            BalanceForecast = entity.BalanceForecast,
            MonthlyDeficitSurplus = entity.MonthlyDeficitSurplus
        };
}

public sealed record AccountBalanceChangedDomainEvent
{
    public CashFlowDomainEvent Current { get; set; }
}
