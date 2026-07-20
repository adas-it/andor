using Andor.Authorizations.Domain;
using Andor.Configurations.Binder.Application;
using Andor.Configurations.Binder.Infrastructure;
using Andor.Configurations.Domain.ValueObjects;
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

    /// <summary>
    /// Seeds the baseline group-to-permission mapping for the Configurations module:
    /// "Administrador" can read configurations, "Administrador Senior" can read and create them.
    /// This is a placeholder until permissions are backed by a real, editable repository.
    /// </summary>
    public static async Task SeedConfigurationPermissionsAsync(this WebApplication app)
    {
        var repository = app.Services.GetRequiredService<IAuthorizationRepository>();

        var permissions = new[]
        {
            new Permission { Name = ConfigurationPermissions.ReadConfiguration, GroupName = "Administrador", Allowed = true },
            new Permission { Name = ConfigurationPermissions.ReadConfiguration, GroupName = "Administrador Senior", Allowed = true },
            new Permission { Name = ConfigurationPermissions.CreateConfiguration, GroupName = "Administrador Senior", Allowed = true },
        };

        foreach (var permission in permissions)
        {
            await repository.SavePermission(permission);
        }
    }
}
