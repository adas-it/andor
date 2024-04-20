using Andor.Infrastructure.Engagement.Budget.Messages.Consumers.IntegrationEventHandlers;
using Andor.Infrastructure.Messaging.Publisher;
using MassTransit;

namespace Andor.Infrastructure.Engagement.Budget.Messages;

public static class BudgetConsumerIntegrationEventHandler
{
    public static IBusRegistrationConfigurator AddBudgetConsumerIntegrations(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<IntegrationEventConsumer>();

        return config;
    }

    public static IServiceBusBusFactoryConfigurator AddBudgetConsumerIntegrationEventHandlerConfig(this IServiceBusBusFactoryConfigurator config, IRegistrationContext context)
    {
        config.SubscriptionEndpoint(
            $"registration-completed-subscription",
            TopicNames.REGISTRATION_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<IntegrationEventConsumer>(context);
            });

        return config;
    }
}
