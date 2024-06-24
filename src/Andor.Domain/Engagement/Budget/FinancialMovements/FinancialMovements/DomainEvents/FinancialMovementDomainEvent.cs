using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;

public record FinancialMovementDomainEvent
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public int Type { get; init; }
    public int Status { get; init; }
    public Guid AccountId { get; init; }
    public decimal Value { get; init; }
    public bool IsDeleted { get; init; }

    public static FinancialMovementDomainEvent FromAggregator(FinancialMovement entity)
        => new FinancialMovementDomainEvent() with
        {
            Id = entity.Id,
            Date = entity.Date,
            Type = entity.Type.Key,
            Status = entity.Status.Key,
            AccountId = entity.Account.Id,
            Value = entity.Value,
            IsDeleted = entity.IsDeleted
        };
}

public sealed record FinancialMovementCreatedDomainEvent
{
    public FinancialMovementDomainEvent Current { get; set; }
}

public sealed record FinancialMovementChangedDomainEvent
{
    public FinancialMovementDomainEvent Old { get; set; }
    public FinancialMovementDomainEvent Current { get; set; }
}

public sealed record FinancialMovementDeletedDomainEvent
{
    public FinancialMovementDomainEvent Current { get; set; }
}
