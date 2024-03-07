using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Infrastructure.Messaging.Consumers.Administrations.Configurations;
using Andor.Infrastructure.Messaging.Publisher.Administrations.Configurations.DomainEventHandlersConfig;
using Andor.Infrastructure.Messaging.RabbitMq;
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
            .GetSection(nameof(RabbitMq))
            .Get<RabbitMq>();

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
                o.QueryDelay = TimeSpan.FromSeconds(200);

                o.UsePostgres().UseBusOutbox();
            });

            x.AddConfigurationsConsumers();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;

                cfg.Host(configs.Host!, "/", h =>
                {
                    h.Username(configs.Username!);
                    h.Password(configs.Password!);
                });

                cfg
                .AddConfigurationsPublisherDomainEventHandlersConfigs()
                .AddConfigurationsConsumerDomainEventHandlersConfigs(context);
            });
        });

        return builder;
    }
}
