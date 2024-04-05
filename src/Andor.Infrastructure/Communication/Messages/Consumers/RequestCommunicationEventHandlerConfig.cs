using Andor.Infrastructure.Communication.Messages.Consumers.Integrations;
using Andor.Infrastructure.Messaging.Publisher;
using MassTransit;

namespace Andor.Infrastructure.Communication.Messages.Consumers;

public static class RequestCommunicationEventHandlerConfig
{
    public static IBusRegistrationConfigurator AddCommunicationConsumers(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<RequestCommunicationEventConsumer>();

        return config;
    }

    public static IServiceBusBusFactoryConfigurator AddCommunicationConsumerDomainEventHandlerConfig(this IServiceBusBusFactoryConfigurator config, IRegistrationContext context)
    {
        config.SubscriptionEndpoint(
            $"registration-created-subscription",
            TopicNames.COMMUNICATION_TOPIC, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer<RequestCommunicationEventConsumer>(context);
            });

        return config;
    }
}
