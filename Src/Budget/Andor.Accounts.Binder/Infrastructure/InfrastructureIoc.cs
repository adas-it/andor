using Andor.Accounts.Binder.Outbox;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Infrastructure;
using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithAccountsInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithAccountsDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddScoped<ICommandsAccountRepository, CommandsAccountRepository>();

        services.AddScoped<ICommandsCurrencyRepository, CommandsCurrencyRepository>();

        services.AddScoped<IOutboxContextProvider, AccountsOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
