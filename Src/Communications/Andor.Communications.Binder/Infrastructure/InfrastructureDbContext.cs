using Andor.Communications.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Communications.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithCommunicationDbContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.AddDbContext<CommunicationContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Communication");

            // Component tests blank this key out (see ComponentTestWebApplicationFactory) and
            // swap in the EF Core InMemory provider instead - skip so both don't end up
            // registered for the same context type, which EF Core rejects outright.
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
        });

    internal static async Task ApplyCommunicationMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CommunicationContext>();

        // Component tests swap this context for the EF Core InMemory provider, which doesn't
        // support migrations at all (IsRelational() is false there) - skip rather than throw.
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
    }
}
