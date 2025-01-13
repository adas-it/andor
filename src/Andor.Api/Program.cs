using Andor.Api.Common.Middlewares;
using Andor.Api.Common.Swagger;
using Andor.Api.WebSocketTest;
using Andor.Application.Common;
using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using Andor.Application.WebSocket;
using Andor.Ioc.Extensions;
using Andor.Ioc.Extensions.Api;
using Andor.Ioc.Extensions.Infrastructures;
using Andor.Ioc.Extensions.Services;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using static Andor.Api.WebSocketTest.WebSocketMessages;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        string? appConfig = Environment.GetEnvironmentVariable("APP_CONFIGURATION");

        if (appConfig is not null)
        {
            builder.Configuration.AddAzureAppConfiguration(appConfig);
        }

        builder.Services
            .AddControllers(options =>
            {
                options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
            })
            .AddNewtonsoftJson();

        builder.AddOpenTelemetry();

        builder.Services.Configure<ApplicationSettings>(builder.Configuration);

        var applicationSettings = builder.Configuration
            .Get<ApplicationSettings>();

        builder.AddDbExtension()
            .AddDbMessagingExtension(applicationSettings)
            .AddApplicationExtensionServices()
            .AddApiExtensionServices()
            .AddServicesExtensionServices();

        var _webSocketMessages = new WebSocketMessages();

        builder.Services.AddSingleton<WebSocketMessages>(_webSocketMessages);
        builder.Services.AddSingleton<IWebSocketMessage>(_webSocketMessages);

        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
                        opts.SerializerOptions.Converters.Add(new ErrorCodeConverter()));
        builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

        builder.Services
            .ConfigureJWT(builder.Configuration);

        builder.Services.AddSwagger(builder.Configuration);

        var app = builder.Build();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI();
        }

        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var clientId = context.Request.Query["id"].ToString();
                    var sessionId = context.Request.Query["sessionId"].ToString();

                    if (string.IsNullOrEmpty(clientId) is false)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        WebSocketConnections.Add(new WebSocketConnection(Guid.Parse(clientId),
                            Guid.Parse(sessionId),
                            webSocket));

                        await EchoWebSocket(Guid.Parse(clientId), Guid.Parse(sessionId), webSocket);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next();
            }
        });

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

    }

    private static async Task EchoWebSocket(Guid clientId, Guid sessionId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        WebSocketConnections.Where(x => x.id == clientId && x.sessionId == sessionId).ToList().ForEach(x => WebSocketConnections.Remove(x));
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}

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