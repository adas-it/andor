using Andor.Domain.Engagement.Budget.Accounts.Invites.DomainEvents;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Producers.Invites.DomainEventHandlersConfig;

public static class InvitesPublisherDomainEventConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddInvitesPublisherDomainEventConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<InviteCreatedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });
        config.Message<GuestNotFoundDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });
        config.Message<GuestFoundDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });
        config.Message<InvitationMadeDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });
        config.Message<InvitationAnsweredDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });

        config.Publish<InviteCreatedDomainEvent>();
        config.Publish<GuestNotFoundDomainEvent>();
        config.Publish<GuestFoundDomainEvent>();
        config.Publish<InvitationMadeDomainEvent>();
        config.Publish<InvitationAnsweredDomainEvent>();

        return config;
    }
}
