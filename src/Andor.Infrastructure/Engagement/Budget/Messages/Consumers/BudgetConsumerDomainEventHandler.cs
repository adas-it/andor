using Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Accounts.DomainEventHandlers;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;
using _accounts = Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Accounts.DomainEventHandlers;
using _invites = Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Invites.DomainEventHandlers;
using _monthlyCash = Andor.Infrastructure.Engagement.Budget.Messages.Consumers.MonthlyCash.DomainEventHandlers;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers;

public static class BudgetConsumerDomainEventHandler
{
    public static IBusRegistrationConfigurator AddBudgetConsumerDomain(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<_accounts.FinancialMovementDomainEventConsumer>();
        config.AddConsumer<_monthlyCash.FinancialMovementDomainEventConsumer>();
        config.AddConsumer<_monthlyCash.CashFlowDomainEventConsumer>();
        config.AddConsumer<_invites.InvitesDomainEventConsumer>();
        config.AddConsumer<UserDomainEventConsumer>();

        return config;
    }

    public static IServiceBusBusFactoryConfigurator AddBudgetConsumerDomainEventHandlerConfig(this IServiceBusBusFactoryConfigurator config, IRegistrationContext context)
    {
        config.SubscriptionEndpoint(
            $"account-subscription",
            TopicNames.ENGAGEMENT_DOMAIN_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<_accounts.FinancialMovementDomainEventConsumer>(context);
            });

        config.SubscriptionEndpoint(
            $"monthly-cash-subscription",
            TopicNames.ENGAGEMENT_DOMAIN_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<_monthlyCash.FinancialMovementDomainEventConsumer>(context);
            });

        config.SubscriptionEndpoint(
            $"invites-subscription",
            TopicNames.ENGAGEMENT_DOMAIN_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<_invites.InvitesDomainEventConsumer>(context);
            });

        config.SubscriptionEndpoint(
            $"user-subscription",
            TopicNames.ENGAGEMENT_DOMAIN_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<UserDomainEventConsumer>(context);
            });


        config.SubscriptionEndpoint(
            $"monthly-cash-subscription",
            TopicNames.CASH_FLOW_DOMAIN_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<_monthlyCash.CashFlowDomainEventConsumer>(context);
            });

        return config;
    }
}
