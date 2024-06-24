namespace Andor.Infrastructure.Engagement.Budget.Messages.Producers.FinancialMovements.DomainEventHandlersConfig;

using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;

public static class FinancialMovementPublisherDomainEventConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddFinancialMovementPublisherDomainEventConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<FinancialMovementCreatedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });

        config.Message<FinancialMovementChangedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });

        config.Message<FinancialMovementDeletedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.ENGAGEMENT_DOMAIN_TOPIC);
        });

        config.Publish<FinancialMovementDeletedDomainEvent>();

        config.Publish<FinancialMovementCreatedDomainEvent>();

        config.Publish<FinancialMovementChangedDomainEvent>();

        return config;
    }
}


