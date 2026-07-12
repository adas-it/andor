namespace Andor.Configurations.Contracts.Responses;

public record ConfigurationOutput(Guid Id, string Name, string Value, string Description,
    DateTime StartDate, DateTime? ExpireDate, KeyValuePair<int, string> State);
