namespace Andor.Infrastructure.Administrations.Messages.Producers.Configurations.DomainEventHandlersConfig;

using Andor.Domain.Administrations.Configurations.Events;
using Andor.Infrastructure.Messaging.Producers;
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


