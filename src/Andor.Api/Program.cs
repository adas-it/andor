using Andor.Api.Common.Middlewares;
using Andor.Api.Common.Swagger;
using Andor.Application.Common;
using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using Andor.Kernel.Extensions;
using Andor.Kernel.Extensions.Api;
using Andor.Kernel.Extensions.Infrastructures;
using Andor.Kernel.Extensions.Services;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("AppConfig");
if (connectionString is not null)
{
    builder.Configuration.AddAzureAppConfiguration(connectionString);
}

builder.AddOpenTelemetry();

builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
    })
    .AddNewtonsoftJson();

builder.AddDbExtension()
    .AddDbMessagingExtension()
    .AddApplicationExtensionServices()
    .AddApiExtensionServices()
    .AddServicesExtensionServices()
    .ConfigureHealthChecks();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
                opts.SerializerOptions.Converters.Add(new ErrorCodeConverter()));
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

builder.Services
    .ConfigureJWT(builder.Configuration);

builder.Services.AddSwagger(builder.Configuration);
builder.Services.Configure<ApplicationSettings>(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.ConfigureHealthChecks();

var configs = app.Services.GetRequiredService<IOptions<ApplicationSettings>>();

app.UseCustomSwagger(configs,
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>());


if (configs.Value?.Cors != null)
{
    app.UseCors(options =>
    {
        options
            .WithOrigins(configs.Value?.Cors?.AllowedOrigins?.ToArray()!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Language");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public static class MyJPIF
{
    public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}