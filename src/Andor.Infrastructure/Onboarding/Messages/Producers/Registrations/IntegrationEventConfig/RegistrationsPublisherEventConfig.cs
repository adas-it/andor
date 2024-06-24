namespace Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.IntegrationEventConfig;

using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;

public static class RegistrationsPublisherEventConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddRegistrationsPublisherEventConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<RegistrationCompleted>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_TOPIC);
        });

        config.Publish<RegistrationCompleted>();

        config.Message<UserCreated>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_TOPIC);
        });

        config.Publish<UserCreated>();


        return config;
    }
}
