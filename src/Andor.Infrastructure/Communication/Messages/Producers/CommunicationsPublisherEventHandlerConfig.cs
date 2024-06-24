using Andor.Application.Dto.Communications.IntegrationsEvents.v1;
using Andor.Infrastructure.Messaging.Producers;
using MassTransit;

namespace Andor.Infrastructure.Communication.Messages.Producers;

public static class CommunicationsPublisherEventHandlerConfig
{
    public static IServiceBusBusFactoryConfigurator AddCommunicationsPublisherEventHandlerConfig(this IServiceBusBusFactoryConfigurator config)
    {
        config.Message<RequestCommunication>(x =>
        {
            x.SetEntityName(TopicNames.COMMUNICATION_TOPIC);
        });

        config.Publish<RequestCommunication>();

        return config;
    }
}
