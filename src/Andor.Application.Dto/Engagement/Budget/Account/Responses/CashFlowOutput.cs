namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public record CashFlowOutput
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal MonthRevenues { get; set; }
    public decimal FinalBalancePreviousMonth { get; set; }
    public decimal ForecastUpcomingRevenues { get; set; }
    public decimal RevenuesBalance { get; set; }
    public decimal Expenses { get; set; }
    public decimal AccountBalance { get; set; }
    public decimal ForecastExpenses { get; set; }
    public decimal BalanceForecast { get; set; }
    public decimal MonthlyDeficitSurplus { get; set; }
}
