using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.ComponentTests.Common;

/// <summary>
/// Base <see cref="WebApplicationFactory{TEntryPoint}"/> for module component tests. Boots the
/// module's real <c>Program.cs</c> - controllers, Akka actors, authorization, migrations/seed
/// calls all run for real - against an isolated EF Core InMemory database instead of the SQL
/// Server/Azure Service Bus configured in appsettings.
/// </summary>
/// <remarks>
/// Because each module's Program.cs runs its migration/seed calls against the same overridden
/// configuration and DI container the test host builds (see WebApplicationFactory's deferred
/// host builder), setting the "Tenants:0:DatabaseType" below to something other than
/// "SQLServer" is enough to make those calls skip touching a real database, while any seeding
/// that only relies on repositories (e.g. Configurations' permission seed) runs normally against
/// our InMemory database.
/// </remarks>
public abstract class ComponentTestWebApplicationFactory<TEntryPoint, TDbContext> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
    where TDbContext : DbContext
{
    private readonly string _databaseName = $"component-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Tenants:0:Name"] = "TenantA",
                ["Tenants:0:DatabaseType"] = "ComponentTest",
                ["Tenants:0:ConnectionString"] = string.Empty,
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.ReplaceDbContextWithInMemory<TDbContext>(_databaseName);
            services.RemoveNonAkkaHostedServices();
            services.ReplaceMessageSenderWithNull();
            services.AddComponentTestAuthentication();

            ConfigureAdditionalTestServices(services);
        });
    }

    /// <summary>
    /// Hook for module-specific overrides (e.g. Communications faking out its SMTP sender).
    /// </summary>
    protected virtual void ConfigureAdditionalTestServices(IServiceCollection services)
    {
    }

    public HttpClient CreateAuthenticatedClient(TestUser? user = null)
    {
        var client = CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        client.SetTestUser(user ?? TestUser.Default);
        return client;
    }
}
