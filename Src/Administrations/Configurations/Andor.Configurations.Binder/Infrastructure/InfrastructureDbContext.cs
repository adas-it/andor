using Andor.Authorizations.Application;
using Andor.Authorizations.Domain;
using Andor.Configurations.Infrastructure.Context;
using Andor.Foundation.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithConfigurationDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ConfigurationContext>((serviceProvider, optionsBuilder) =>
        {
            var currentUserService = serviceProvider.GetService<ICurrentUserService>();
            var tenantService = serviceProvider.GetService<ITenantService>();

            if (currentUserService is null || tenantService is null)
                return;

            var tenantName = currentUserService.GetCurrentUser().Tenant;
            var tenant = tenantService.GetTenants()
                .FirstOrDefault(x => x.Name.Contains(tenantName));

            if (tenant?.DatabaseType == TenantService.SQLServer)
            {
#if DEBUG
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
#endif
                optionsBuilder.UseSqlServer(tenant.ConnectionString);
            }
        });

        return services;
    }
}
