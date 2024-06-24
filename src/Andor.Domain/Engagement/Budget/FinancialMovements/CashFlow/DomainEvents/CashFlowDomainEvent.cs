namespace Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.DomainEvents;

public record CashFlowDomainEvent
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal AccountBalance { get; set; }
    public Guid AccountId { get; set; }

    public static CashFlowDomainEvent FromAggregator(CashFlow entity)
        => new CashFlowDomainEvent() with
        {
            Id = entity.Id,
            Year = entity.Year,
            Month = entity.Month,
            AccountBalance = entity.AccountBalance,
            AccountId = entity.AccountId
        };
}

public sealed record AccountBalanceChangedDomainEvent
{
    public CashFlowDomainEvent Current { get; set; }
}
