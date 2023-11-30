namespace Family.Budget.Api.Common.Hangfire;

using global::Hangfire;
using global::Hangfire.MemoryStorage;
using global::Hangfire.PostgreSql;
using Microsoft.Extensions.Options;
using Family.Budget.Application.Common;
using Family.Budget.Api.Middlewares;

public static class HangfireExtension
{
    public static IServiceCollection AddHangfire(this IServiceCollection services
        , IConfiguration configuration
        , string env)
    {
        var conn = configuration.GetConnectionString("HangfireConnection");

        if (env != "Test" && conn is not null)
        {
            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(conn, new PostgreSqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(10),
            }));
        }
        else
        {
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });
        }

        services.AddHangfireServer();

        return services;
    }

    public static IApplicationBuilder UseCustomHangfire(this IApplicationBuilder app,
        IOptions<ApplicationSettings> appConfig)
    {

        var options = new DashboardOptions
        {
            Authorization = new[] {
                new DashboardAuthorization(new[]
                    {
                        new HangfireUserCredentials(
                            appConfig.Value?.HangfireDashboard?.User ?? "admin",
                            appConfig.Value?.HangfireDashboard?.Password ?? "Pa$$w0rd")
                    })
                }
        };

        app.UseHangfireDashboard("/hangfire", options);

        return app;
    }
}

