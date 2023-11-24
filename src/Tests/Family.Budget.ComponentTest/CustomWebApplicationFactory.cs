namespace Family.Budget.ComponentTest;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.ComponentTest.Utils;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
{
    public TopicMessageTestHelper message;

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

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<PrincipalContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<PrincipalContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDatabase");
            });

            var descriptorEvent = services.Where(
                    d => d.ServiceType ==
                        typeof(IMessageSenderInterface)).ToList();

            if (descriptorEvent != null)
            {
                descriptorEvent.ForEach(e => services.Remove(e));
            }

            services.AddSingleton<IMessageSenderInterface>(message);

            var sp = services.BuildServiceProvider();

            // Create scope for db initialization
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<PrincipalContext>();
            db.Database.EnsureCreated();
        });
    }
}