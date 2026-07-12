using Andor.Configurations.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithConfigurationDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ConfigurationContext>();

        return services;
    }
}
