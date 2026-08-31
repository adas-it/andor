using Andor.Users.Application.Interfaces;
using Andor.Users.Binder.Outbox;
using Andor.Users.Infrastructure;
using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Andor.Users.Domain.Users.Repositories;

namespace Andor.Users.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithUserInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithUserDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddScoped<IQueriesUserRepository, QueriesUserRepository>();

        services.AddScoped<ICommandsUserRepository, CommandsUserRepository>();

        services.AddScoped<IOutboxContextProvider, UserOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
