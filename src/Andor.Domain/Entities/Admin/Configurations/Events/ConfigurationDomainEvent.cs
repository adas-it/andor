using Andor.Domain.SeedWork;

namespace Andor.Domain.Entities.Admin.Configurations.Events;

public record ConfigurationCreatedDomainEvent(Configuration configuration)
    : DomainEventBase<Configuration>(nameof(ConfigurationCreatedDomainEvent), configuration);

public record ConfigurationUpdatedDomainEvent(Configuration configuration)
    : DomainEventBase<Configuration>(nameof(ConfigurationUpdatedDomainEvent), configuration);

public record ConfigurationDeletedDomainEvent(Configuration configuration)
    : DomainEventBase<Configuration>(nameof(ConfigurationDeletedDomainEvent), configuration);
