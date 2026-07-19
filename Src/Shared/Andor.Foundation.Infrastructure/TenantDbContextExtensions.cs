using Andor.Authorizations.Domain;
using Andor.Foundation.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Foundation.Infrastructure;

/// <summary>
/// Generic, tenant-aware registration and migration helpers for any
/// <see cref="PrincipalContext"/>-derived context. Modules reuse these instead of
/// re-implementing the same multi-tenant SQL Server wiring.
/// </summary>
public static class TenantDbContextExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TContext"/> resolving the SQL Server connection string
    /// from the current user's tenant at request time.
    /// </summary>
    public static IServiceCollection WithTenantDbContext<TContext>(this IServiceCollection services)
        where TContext : PrincipalContext
    {
        services.AddDbContext<TContext>((serviceProvider, optionsBuilder) =>
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

    /// <summary>
    /// Applies EF Core migrations of <typeparamref name="TContext"/> against every configured
    /// SQL Server tenant database. The <paramref name="contextFactory"/> builds a context from
    /// the tenant-specific options.
    /// </summary>
    public static async Task ApplyTenantMigrationsAsync<TContext>(
        this IServiceProvider serviceProvider,
        Func<DbContextOptions<TContext>, TContext> contextFactory)
        where TContext : PrincipalContext
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();

        foreach (var tenant in tenantService.GetTenants())
        {
            if (tenant.DatabaseType != TenantService.SQLServer)
                continue;

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            _ = optionsBuilder.UseSqlServer(tenant.ConnectionString);

            await using var db = contextFactory(optionsBuilder.Options);
            await db.Database.MigrateAsync();
        }
    }
}
