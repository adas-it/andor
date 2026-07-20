using Andor.Communications.Application.Interfaces;
using Andor.Communications.Binder.Outbox;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Infrastructure;
using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Communications.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithCommunicationInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithCommunicationDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddScoped<ICommandsRuleRepository, CommandsRuleRepository>();

        services.AddScoped<IOutboxContextProvider, CommunicationOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
