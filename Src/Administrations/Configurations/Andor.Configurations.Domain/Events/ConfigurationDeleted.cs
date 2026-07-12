using Andor.Foundation.Domain.Events;

namespace Andor.Configurations.Domain.Events;

public record ConfigurationDeleted : DomainEvent
{
    public static ConfigurationDeleted FromConfiguration(Configuration configuration, Guid userId)
        => new ConfigurationDeleted() with
        {
            Id = configuration.Id,
            UserId = userId
        };
}
