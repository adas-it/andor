using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Domain.Repositories;
using Andor.Configurations.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithConfigurationInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithConfigurationDbContext(configuration);

        services.AddScoped<IQueriesConfigurationRepository,
            QueriesConfigurationRepository>();

        services.AddScoped<ICommandsConfigurationRepository,
            CommandsConfigurationRepository>();

        return services;
    }
}
