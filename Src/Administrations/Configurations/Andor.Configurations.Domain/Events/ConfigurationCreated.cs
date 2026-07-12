using Andor.Foundation.Domain.Events;

namespace Andor.Configurations.Domain.Events;

public record ConfigurationCreated : DomainEvent
{
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public string Description { get; init; } = "";
    public DateTime StartDate { get; init; }
    public DateTime? ExpireDate { get; init; }

    public static ConfigurationCreated FromConfiguration(Configuration Configuration, Guid UserId)
        => new ConfigurationCreated() with
        {
            Id = Configuration.Id,
            Name = Configuration.Name,
            Value = Configuration.Value,
            Description = Configuration.Description,
            StartDate = Configuration.StartDate,
            ExpireDate = Configuration.ExpireDate,
            UserId = UserId
        };
}
