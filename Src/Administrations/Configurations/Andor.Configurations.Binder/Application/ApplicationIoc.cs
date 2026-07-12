using Andor.Configurations.Application;
using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Domain;
using Andor.Foundation.Binder;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder.Application;

internal static class ApplicationIoc
{
    public static IServiceCollection WithConfigurationApplication(this IServiceCollection services)
    {
        services.AddSingleton<IAkkaModule, ApplicationAkkaModule>();

        services.AddScoped<IConfigurationValidator, ConfigurationValidator>();

        services.AddScoped<IConfigurationQueriesService, ConfigurationQueriesService>();

        services.AddScoped<IConfigurationCommandsService, ConfigurationCommandsService>();

        return services;
    }
}
