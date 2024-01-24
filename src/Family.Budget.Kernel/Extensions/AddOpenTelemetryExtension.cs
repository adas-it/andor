using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Family.Budget.Kernel.Extensions;
public static class AddOpenTelemetryExtension
{
    private static readonly string SourceName = "PersonalTrainer";

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(op =>
                op.AddHttpClientInstrumentation()
                .AddMeter("Andor")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(SourceName).AddTelemetrySdk())
                .AddConsoleExporter()
                .AddPrometheusExporter()
                /*.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri("/metrics");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                })*/
                )
            .WithTracing(op =>
                op.AddSource(SourceName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(SourceName)
                .AddTelemetrySdk())
                .AddSqlClientInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.RecordException = true;
                })
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = (req) => !req.Request.Path.ToUriComponent().Contains("index.html", StringComparison.OrdinalIgnoreCase)
                        && !req.Request.Path.ToUriComponent().Contains("swagger", StringComparison.OrdinalIgnoreCase);
                })
                .AddHttpClientInstrumentation()
                .AddConsoleExporter()
                /*.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri("/metrics");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                })*/
                );

        return services;
    }
}
