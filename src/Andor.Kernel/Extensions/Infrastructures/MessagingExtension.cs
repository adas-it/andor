using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Application.Engagement.Budget.Invites.Saga;
using Andor.Infrastructure.Administrations.Messages.Consumers.Configurations;
using Andor.Infrastructure.Administrations.Messages.Producers.Configurations.DomainEventHandlersConfig;
using Andor.Infrastructure.Communication.Messages.Consumers;
using Andor.Infrastructure.Communication.Messages.Producers;
using Andor.Infrastructure.Engagement.Budget.Messages.Consumers;
using Andor.Infrastructure.Engagement.Budget.Messages.Producers.CashFlow.DomainEventHandlersConfig;
using Andor.Infrastructure.Engagement.Budget.Messages.Producers.FinancialMovements.DomainEventHandlersConfig;
using Andor.Infrastructure.Engagement.Budget.Messages.Producers.Invites.DomainEventHandlersConfig;
using Andor.Infrastructure.Engagement.Budget.Messages.Producers.Users.DomainEventHandlersConfig;
using Andor.Infrastructure.Messaging.Producers;
using Andor.Infrastructure.Onboarding.Messages.Consumers.Registrations;
using Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.DomainEventConfig;
using Andor.Infrastructure.Onboarding.Messages.Producers.Registrations.IntegrationEventConfig;
using Andor.Infrastructure.Repositories.Context;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Kernel.Extensions.Infrastructures;

public static class MessagingExtension
{
    public static WebApplicationBuilder AddDbMessagingExtension(this WebApplicationBuilder builder, ApplicationSettings applicationSettings)
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
                o.QueryDelay = TimeSpan.FromMilliseconds(1000);

                o.UsePostgres()
                .UseBusOutbox();
            });
            x.AddSagaStateMachine<InviteSaga, InviteSagaState>()
            .EntityFrameworkRepository(r =>
             {
                 r.ExistingDbContext<PrincipalContext>();

                 r.UsePostgres();
             });
            x.AddConfigureEndpointsCallback((_, cfg) =>
            {
                if (cfg is IServiceBusReceiveEndpointConfigurator sb)
                {
                    sb.ConfigureDeadLetterQueueDeadLetterTransport();
                    sb.ConfigureDeadLetterQueueErrorTransport();
                }
            });

            x
            .AddConfigurationsConsumers()
            .AddRegistrationsConsumers()
            .AddCommunicationConsumers()
            .AddBudgetConsumerIntegrations()
            .AddBudgetConsumerDomain();

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

                cfg
                .AddCommunicationsPublisherEventHandlerConfig()
                .AddCommunicationConsumerDomainEventHandlerConfig(context);

                cfg
                .AddBudgetConsumerIntegrationEventHandlerConfig(context);

                cfg
                .AddBudgetConsumerDomainEventHandlerConfig(context)
                .AddFinancialMovementPublisherDomainEventConfig();

                cfg.AddCashFlowPublisherDomainEventConfig();

                cfg.AddInvitesPublisherDomainEventConfig();

                cfg.AddUsersPublisherDomainEventConfig();

                cfg.DeployPublishTopology = true;
            });
        });

        return builder;
    }
}
