using Andor.Accounts.Infrastructure.Context;
using Andor.Foundation.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithAccountsDbContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.WithTenantDbContext<AccountsContext>();

    internal static Task ApplyAccountsMigrationsAsync(this IServiceProvider serviceProvider)
        => serviceProvider.ApplyTenantMigrationsAsync<AccountsContext>(
            options => new AccountsContext(options));
}
