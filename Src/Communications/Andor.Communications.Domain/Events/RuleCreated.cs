using Andor.Foundation.Domain.Events;

namespace Andor.Communications.Domain.Events;

public record RuleCreated : DomainEvent
{
    public string Name { get; init; } = "";
    public string Type { get; init; } = "";
    public DateTime CreatedAt { get; init; }

    public static RuleCreated FromRule(Rule rule, Guid userId)
        => new RuleCreated() with
        {
            Id = rule.Id,
            Name = rule.Name,
            Type = rule.Type.Name,
            CreatedAt = rule.CreatedAt,
            UserId = userId
        };
}
