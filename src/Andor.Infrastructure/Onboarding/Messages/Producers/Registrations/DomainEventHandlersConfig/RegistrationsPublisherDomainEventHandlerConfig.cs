namespace Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.DomainEventHandlersConfig;

using Andor.Domain.Entities.Onboarding.Registrations.DomainEvents;
using Andor.Infrastructure.Messaging.Publisher;
using MassTransit;

public static class RegistrationsPublisherDomainEventHandlerConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddRegistrationsPublisherDomainEventHandlerConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<RegistrationCreatedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });

        config.Publish<RegistrationCreatedDomainEvent>();

        config.Message<RegistrationCodeChangedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });

        config.Publish<RegistrationCodeChangedDomainEvent>();

        config.Message<RegistrationCompletedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });

        config.Publish<RegistrationCompletedDomainEvent>();

        return config;
    }
}
