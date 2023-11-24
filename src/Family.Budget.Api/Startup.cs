namespace Family.Budget.Api;

using Asp.Versioning.ApiExplorer;
using Family.Budget.Api.Common.Hangfire;
using Family.Budget.Api.Common.Middlewares;
using Family.Budget.Api.Extensions;
using Family.Budget.Api.Swagger;
using Family.Budget.Application.Common;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Kernel.Extensions;
using Hangfire;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;

public class Startup
{
    private string env;
    public readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _currentEnvironment;
    private IConfigurationRefresher? _refresher;

    public Startup(IWebHostEnvironment environment)
    {
        var conf = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json");

        var conn = Environment.GetEnvironmentVariable("AppConfig");

        if (!String.IsNullOrEmpty(conn))
        {
            conf.AddAzureAppConfiguration(options =>
            {
                options.Connect(conn);

                options.UseFeatureFlags(e =>
                    e.CacheExpirationInterval = TimeSpan.FromSeconds(40));

                _refresher = options.GetRefresher();
            });
        }

        env = Environment.GetEnvironmentVariable("ENVIRONMENT_CONFIG") ?? "";

        if (!String.IsNullOrEmpty(env))
        {
            conf.AddJsonFile($"appsettings.{env}.json");
        }

        _configuration = conf.Build();
        _currentEnvironment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDbContexts(_configuration)
            .AddUseCases()
            .ConfigureApplicationInfrastructure()
            .AddGlobalExceptionHandlerMiddleware()
            .AddInfraServices()
            .AddHttpContextAccessor()
            .AddServices()
            .AddOpenTelemetry(_configuration)
            .AddHangfire(_configuration, env)
            .AddControllers(options => { })
            .AddJsonOptions(opts =>
                opts.JsonSerializerOptions.Converters.Add(new ErrorCodeConverter()));

        services
            .ConfigureJWT(_configuration, _currentEnvironment.IsDevelopment(), _currentEnvironment);
        
        services
            .AddHttpClientsAsync(_configuration);

        services
            .AddSwagger(_configuration);

        services.Configure<ApplicationSettings>(_configuration);

        if (_refresher is not null)
        {
            services.AddSingleton(_refresher!);
        }
    }

    public void Configure(IApplicationBuilder app,
        IWebHostEnvironment env,
        IOptions<ApplicationSettings> appConfig,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseCustomSwagger(appConfig, apiVersionDescriptionProvider);
        app.UseCustomHangfire(appConfig);

        if (appConfig.Value?.Cors != null)
        {
            app.UseCors(options =>
            {
                options
                    .WithOrigins(appConfig.Value?.Cors?.AllowedOrigins?.ToArray()!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders(ApiHttpHeaders.CONTENT_LANGUAGE);
            });
        }

        app.UseGlobalExceptionHandlerMiddleware();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}