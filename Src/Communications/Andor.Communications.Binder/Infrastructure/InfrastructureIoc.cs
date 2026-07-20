using Andor.Application.Common;
using Andor.Application.Communications.Interfaces;
using Andor.Communications.Application.Interfaces;
using Andor.Communications.Binder.Outbox;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Infrastructure;
using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Andor.Infrastructure.Communication.Gateway;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Andor.Communications.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithCommunicationInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithCommunicationDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddOptions<ApplicationSettings>()
            .Bind(configuration.GetSection("ApplicationSettings"));

        services.AddScoped<ISMTP, Smtp>();

        services.AddScoped<ICommandsRuleRepository, CommandsRuleRepository>();

        services.AddScoped<IOutboxContextProvider, CommunicationOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
