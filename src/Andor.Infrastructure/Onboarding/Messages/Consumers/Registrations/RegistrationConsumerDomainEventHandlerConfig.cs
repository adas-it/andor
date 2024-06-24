namespace Andor.Infrastructure.Onboarding.Messages.Consumers.Registrations;

using Andor.Infrastructure.Messaging.Producers;
using Andor.Infrastructure.Onboarding.Messages.Consumers.Registrations.DomainEventHandlers;
using MassTransit;

public static class RegistrationConsumerDomainEventHandlerConfig
{
    public static IBusRegistrationConfigurator AddRegistrationsConsumers(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<RegistrationDomainEventConsumer>();

        return config;
    }

    public static IServiceBusBusFactoryConfigurator AddRegistrationConsumerDomainEventHandlerConfig(this IServiceBusBusFactoryConfigurator config, IRegistrationContext context)
    {
        config.SubscriptionEndpoint(
            $"registration-created-subscription",
            TopicNames.REGISTRATION_DOMAIN_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<RegistrationDomainEventConsumer>(context);
            });

        return config;
    }
}
