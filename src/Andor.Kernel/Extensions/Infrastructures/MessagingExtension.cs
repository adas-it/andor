using Andor.Application.Common.Interfaces;
using Andor.Infrastructure.Administrations.Messaging.Consumers.Configurations;
using Andor.Infrastructure.Administrations.Messaging.Publisher.Configurations.DomainEventHandlersConfig;
using Andor.Infrastructure.Communication.Messages.Consumers;
using Andor.Infrastructure.Communication.Messages.Producers;
using Andor.Infrastructure.Messaging.RabbitMq;
using Andor.Infrastructure.Onboarding.Messages.Consumers.Registrations;
using Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.DomainEventHandlersConfig;
using Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.IntegrationEventConfig;
using Andor.Infrastructure.Repositories.Context;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Kernel.Extensions.Infrastructures;

public static class MessagingExtension
{
    public static WebApplicationBuilder AddDbMessagingExtension(this WebApplicationBuilder builder)
    {
        var configs = builder.Configuration
            .GetConnectionString("ServiceBus");

        if (configs == null)
        {
            return builder;
        }

        builder.Services.AddScoped<IMessageSenderInterface, SendMessagePublisher>();

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<PrincipalContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromMilliseconds(500);

                o.UsePostgres()
                .UseBusOutbox();
            });

            x.AddConfigureEndpointsCallback((_, cfg) =>
            {
                if (cfg is IServiceBusReceiveEndpointConfigurator sb)
                {
                    sb.ConfigureDeadLetterQueueDeadLetterTransport();
                    sb.ConfigureDeadLetterQueueErrorTransport();
                }
            });

            x.AddConfigurationsConsumers()
                .AddRegistrationsConsumers()
                .AddCommunicationConsumers();

            x.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(configs);

                cfg
                .AddConfigurationsPublisherDomainEventHandlersConfig()
                .AddConfigurationsConsumerDomainEventHandlersConfig(context);
                cfg
                .AddRegistrationsPublisherDomainEventConfig()
                .AddRegistrationsPublisherEventConfig()
                .AddRegistrationConsumerDomainEventHandlerConfig(context);

                cfg.AddCommunicationsPublisherEventHandlerConfig()
                .AddCommunicationConsumerDomainEventHandlerConfig(context);

                cfg.DeployPublishTopology = true;
            });
        });

        return builder;
    }
}
