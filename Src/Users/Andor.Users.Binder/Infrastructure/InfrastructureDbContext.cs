using Andor.Users.Infrastructure.Context;
using Andor.Foundation.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Users.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithUserDbContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.WithTenantDbContext<UserContext>();

    internal static Task ApplyUserMigrationsAsync(this IServiceProvider serviceProvider)
        => serviceProvider.ApplyTenantMigrationsAsync<UserContext>(
            options => new UserContext(options));
}
