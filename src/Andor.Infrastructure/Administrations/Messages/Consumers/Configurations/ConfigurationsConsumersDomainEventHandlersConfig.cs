namespace Andor.Infrastructure.Administrations.Messaging.Consumers.Configurations;
using Andor.Infrastructure.Administrations.Messaging.Consumers.Configurations.DomainEventHandlers;
using Andor.Infrastructure.Messaging.Publisher;
using MassTransit;
public static class ConfigurationsConsumersDomainEventHandlersConfig
{
    public static IBusRegistrationConfigurator AddConfigurationsConsumers(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<ConfigurationCreatedDomainEventConsumer>();

        return config;
    }

    public static IServiceBusBusFactoryConfigurator AddConfigurationsConsumerDomainEventHandlersConfig(this IServiceBusBusFactoryConfigurator config, IRegistrationContext context)
    {
        config.SubscriptionEndpoint(
            $"configuration-created-subscription",
            TopicNames.CONFIGURATION_DOMAIN_TOPIC, x =>
        {
            x.ConfigureConsumeTopology = false;

            x.Consumer<ConfigurationCreatedDomainEventConsumer>(context);
        });

        return config;
    }
}
