namespace Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.DomainEventHandlersConfig;

using Andor.Domain.Onboarding.Registrations.DomainEvents;
using Andor.Domain.Onboarding.Users.DomainEvents;
using Andor.Infrastructure.Messaging.Publisher;
using MassTransit;

public static class RegistrationsPublisherDomainEventConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddRegistrationsPublisherDomainEventConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<RegistrationCreatedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });

        config.Message<RegistrationCodeChangedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });

        config.Message<RegistrationCompletedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });

        config.Publish<RegistrationCodeChangedDomainEvent>();
        config.Publish<RegistrationCompletedDomainEvent>();
        config.Publish<RegistrationCreatedDomainEvent>();


        config.Message<UserCreatedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.REGISTRATION_DOMAIN_TOPIC);
        });
        config.Publish<UserCreatedDomainEvent>();

        return config;
    }
}
