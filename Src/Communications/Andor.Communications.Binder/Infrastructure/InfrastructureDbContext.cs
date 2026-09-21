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
            options.UseSqlServer(configuration.GetConnectionString("Communication")));

    internal static async Task ApplyCommunicationMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CommunicationContext>();

        await context.Database.MigrateAsync();
    }
}
