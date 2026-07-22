using Andor.Foundation.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Andor.ComponentTests.Common;

public static class ComponentTestServiceCollectionExtensions
{
    /// <summary>
    /// Swaps a module's tenant-resolved SQL Server <typeparamref name="TContext"/> registration
    /// for an isolated EF Core InMemory database.
    /// </summary>
    public static IServiceCollection ReplaceDbContextWithInMemory<TContext>(this IServiceCollection services,
        string databaseName)
        where TContext : DbContext
    {
        services.RemoveAll(typeof(DbContextOptions<TContext>));

        services.AddDbContext<TContext>(options => options.UseInMemoryDatabase(databaseName));

        return services;
    }

    public static IServiceCollection ReplaceMessageSenderWithNull(this IServiceCollection services)
    {
        services.RemoveAll(typeof(IMessageSenderInterface));
        services.AddSingleton<IMessageSenderInterface, NullMessageSender>();

        return services;
    }

    /// <summary>
    /// Removes every registered <see cref="IHostedService"/> that isn't part of Akka.Hosting:
    /// the Outbox dispatcher and any Service Bus queue/topic consumers. Left running, they try to
    /// reach a real Azure Service Bus namespace on startup and, since ASP.NET Core's default
    /// HostOptions stops the whole host on an unhandled BackgroundService exception, would take
    /// the entire test host down with them.
    /// </summary>
    public static IServiceCollection RemoveNonAkkaHostedServices(this IServiceCollection services)
    {
        var toRemove = services
            .Where(d => d.ServiceType == typeof(IHostedService)
                        && d.ImplementationType?.Namespace is { } ns
                        && !ns.StartsWith("Akka", StringComparison.Ordinal))
            .ToList();

        foreach (var descriptor in toRemove)
        {
            services.Remove(descriptor);
        }

        return services;
    }

    public static IServiceCollection ReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.RemoveAll(typeof(TService));
        services.AddSingleton<TService, TImplementation>();

        return services;
    }

    public static IServiceCollection ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.RemoveAll(typeof(TService));
        services.AddScoped<TService, TImplementation>();

        return services;
    }
}
