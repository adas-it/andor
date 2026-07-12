using Andor.Foundation.Domain.Events;

namespace Andor.Configurations.Domain.Events;

public record ConfigurationDeactivated : DomainEvent
{
    public DateTime? ExpireDate { get; init; }

    public static ConfigurationDeactivated FromConfiguration(Configuration Configuration, Guid userId)
        => new ConfigurationDeactivated() with
        {
            Id = Configuration.Id,
            ExpireDate = Configuration.ExpireDate,
            UserId = userId
        };
}
