namespace Andor.Kernel.Extensions.Api;

using Andor.Application.Common;
using Azure.Monitor.OpenTelemetry.Exporter;
using HealthChecks.OpenTelemetry.Instrumentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class AddOpenTelemetryExtension
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var configs = builder.Configuration
            .GetSection(nameof(OpenTelemetryConfig))
            .Get<OpenTelemetryConfig>();

        var _applicationInsights = builder.Configuration.GetConnectionString("ApplicationInsights");

        if (configs is null || _applicationInsights is null)
        {
            return builder;
        }

        Action<ResourceBuilder> configureResource = r => r.AddService(
        serviceName: "Andor.api",
        serviceVersion: "1.0",
        serviceInstanceId: Environment.MachineName);

        builder.Services.AddOpenTelemetry()
        .ConfigureResource(configureResource)
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddNpgsql()
            .AddAzureMonitorTraceExporter(o =>
            {
                o.ConnectionString = _applicationInsights;
            });
        })
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddHealthChecksInstrumentation(o =>
            {
                o.StatusGaugeName = configs.StatusGaugeName!;
                o.DurationGaugeName = configs.DurationGaugeName!;
            })
            .AddAzureMonitorMetricExporter(o =>
            {
                o.ConnectionString = _applicationInsights;
            }));

        builder.Logging.ClearProviders();

        builder.Logging
        .AddOpenTelemetry(options =>
        {
            options.AddAzureMonitorLogExporter(o =>
            {
                o.ConnectionString = _applicationInsights;
            });
        });


        return builder;
    }
}

