using Andor.Configurations.Infrastructure.Context;
using Andor.Foundation.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithConfigurationDbContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.WithTenantDbContext<ConfigurationContext>();

    internal static Task ApplyConfigurationMigrationsAsync(this IServiceProvider serviceProvider)
        => serviceProvider.ApplyTenantMigrationsAsync<ConfigurationContext>(
            options => new ConfigurationContext(options));
}
