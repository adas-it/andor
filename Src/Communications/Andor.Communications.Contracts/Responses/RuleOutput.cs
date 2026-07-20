namespace Andor.Communications.Contracts.Responses;

public record RuleOutput(Guid Id, string Name, KeyValuePair<int, string> Type, DateTime CreatedAt);
