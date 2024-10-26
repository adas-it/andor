using Andor.Application.Communications.Interfaces;
using Andor.Component.Tests.Utils;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Andor.Component.Tests;

public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:14.3")
    .WithPassword("P@55w0rd")
    .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var conf = new ConfigurationBuilder();

        conf.AddJsonFile("secrets.json");

        builder.UseConfiguration(conf.Build());

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<PrincipalContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            var descriptorEvent = services.Where(
                    d => d.ServiceType ==
                        typeof(ISMTP)).ToList();

            if (descriptorEvent != null)
            {
                descriptorEvent.ForEach(e => services.Remove(e));
            }

            services.AddSingleton<ISMTP>(SMTP_Test.Instance());

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PrincipalContext>();
            db.Database.Migrate();
        });
    }

    public Task InitializeAsync() => _dbContainer.StartAsync();

    public new Task DisposeAsync() => _dbContainer.StopAsync();
}