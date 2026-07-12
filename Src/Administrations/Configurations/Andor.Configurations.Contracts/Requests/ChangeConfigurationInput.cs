namespace Andor.Configurations.Contracts.Requests;

public record ChangeConfigurationInput(string Name, string Value, string Description,
    DateTime StartDate, DateTime? ExpireDate);
