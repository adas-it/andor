namespace Andor.Infrastructure.Administrations.Messaging.Publisher.Configurations.DomainEventHandlersConfig;

using Andor.Domain.Entities.Admin.Configurations.Events;
using Andor.Infrastructure.Messaging.Publisher;
using MassTransit;

public static class ConfigurationsPublisherDomainEventHandlersConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddConfigurationsPublisherDomainEventHandlersConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<ConfigurationCreatedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.CONFIGURATION_DOMAIN_TOPIC);
        });
        config.Publish<ConfigurationCreatedDomainEvent>();

        config.Message<ConfigurationDeletedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.CONFIGURATION_DOMAIN_TOPIC);
        });
        config.Publish<ConfigurationDeletedDomainEvent>();

        return config;
    }
}


