using Andor.Communications.Infrastructure.Context;
using Andor.Foundation.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Communications.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithCommunicationDbContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.WithTenantDbContext<CommunicationContext>();

    internal static Task ApplyCommunicationMigrationsAsync(this IServiceProvider serviceProvider)
        => serviceProvider.ApplyTenantMigrationsAsync<CommunicationContext>(
            options => new CommunicationContext(options));
}
