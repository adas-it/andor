namespace Andor.Configurations.Contracts.Requests;

public record CreateConfigurationInput(string Name, string Value, string Description,
    DateTime StartDate, DateTime? ExpireDate, bool Force);
