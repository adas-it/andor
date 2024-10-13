using Andor.Domain.Engagement.Budget.Accounts.Users.DomainEvents;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Producers.Users.DomainEventHandlersConfig
{
    public static class UserPublisherDomainEventConfig
    {
        public static IServiceBusBusFactoryConfigurator
        AddUsersPublisherDomainEventConfig(this IServiceBusBusFactoryConfigurator config)
        {
            config.Message<UserCreatedDomainEvent>(x =>
            {
                x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
            });

            config.Publish<UserCreatedDomainEvent>();

            return config;
        }
    }
}
