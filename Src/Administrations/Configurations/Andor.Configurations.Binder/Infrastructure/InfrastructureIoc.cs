using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Binder.Outbox;
using Andor.Configurations.Domain.Repositories;
using Andor.Configurations.Infrastructure;
using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithConfigurationInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithConfigurationDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddScoped<IQueriesConfigurationRepository,
            QueriesConfigurationRepository>();

        services.AddScoped<ICommandsConfigurationRepository,
            CommandsConfigurationRepository>();

        services.AddScoped<IOutboxContextProvider, ConfigurationOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
