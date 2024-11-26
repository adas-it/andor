namespace Andor.Kernel.Extensions.Api;

using Andor.Application.Common;
using Azure.Monitor.OpenTelemetry.Exporter;
using HealthChecks.OpenTelemetry.Instrumentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
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

        if (configs is null)
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
            .AddNpgsql();

            if (string.IsNullOrEmpty(configs.ApplicationInsights) is false)
            {
                tracing.AddAzureMonitorTraceExporter(o =>
                {
                    o.ConnectionString = configs.ApplicationInsights;
                });
            }

            if (string.IsNullOrEmpty(configs.Endpoint) is false)
            {
                tracing.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(configs.Endpoint!);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
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
            });

            if (string.IsNullOrEmpty(configs.ApplicationInsights) is false)
            {
                metrics.AddAzureMonitorMetricExporter(o =>
                {
                    o.ConnectionString = configs.ApplicationInsights;
                });
            }

            if (string.IsNullOrEmpty(configs.Endpoint) is false)
            {
                metrics.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(configs.Endpoint!);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });

        builder.Logging.ClearProviders();

        builder.Logging
        .AddOpenTelemetry(options =>
        {
            if (string.IsNullOrEmpty(configs.ApplicationInsights) is false)
            {
                options.AddAzureMonitorLogExporter(o =>
                {
                    o.ConnectionString = configs.ApplicationInsights;
                });
            }

            if (string.IsNullOrEmpty(configs.Endpoint) is false)
            {
                options.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(configs.Endpoint!);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });


        return builder;
    }
}

