using Andor.Configurations.Binder.Application;
using Andor.Configurations.Binder.Infrastructure;
using Andor.Configurations.RestApi;
using Andor.Foundation.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder UseConfigurations(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        _ = builder.Services.AddScoped<ITenantService, TenantService>();

        _ = builder.Services.UseApi()
            .WithConfigurationApplication()
            .WithConfigurationInfrastructure(configuration);

        return builder;
    }

    public static Task ApplyConfigurationMigrationsAsync(this WebApplication app)
        => app.Services.ApplyConfigurationMigrationsAsync();
}
