namespace Andor.Infrastructure.Engagement.Budget.Messages.Producers.FinancialMovements.DomainEventHandlersConfig;

using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.DomainEvents;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;

public static class CashFlowPublisherDomainEventConfig
{
    public static IServiceBusBusFactoryConfigurator
        AddCashFlowPublisherDomainEventConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<AccountBalanceChangedDomainEvent>(x =>
        {
            x.SetEntityName(TopicNames.CASH_FLOW_DOMAIN_TOPIC);
        });

        config.Publish<AccountBalanceChangedDomainEvent>();

        return config;
    }
}


