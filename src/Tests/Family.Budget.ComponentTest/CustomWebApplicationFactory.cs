namespace Family.Budget.ComponentTest;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.ComponentTest.Utils;
using Family.Budget.Infrastructure.Repositories.Context;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    public TopicMessageTestHelper message;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:14.3")
    .WithPassword("P@55w0rd")
    .Build();

    protected override IHostBuilder CreateHostBuilder()
    {
        return base.CreateHostBuilder()
            .UseEnvironment("Test");
    }

    public CustomWebApplicationFactory()
    {
        message = new TopicMessageTestHelper();

    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ENVIRONMENT_CONFIG", "Test");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<PrincipalContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            var descriptorEvent = services.Where(
                    d => d.ServiceType ==
                        typeof(IMessageSenderInterface)).ToList();

            if (descriptorEvent != null)
            {
                descriptorEvent.ForEach(e => services.Remove(e));
            }

            services.AddSingleton<IMessageSenderInterface>(message);

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PrincipalContext>();
            db.Database.Migrate();
        });
    }

    public Task InitializeAsync() => _dbContainer.StartAsync();

    public new Task DisposeAsync() => _dbContainer.StopAsync();
}