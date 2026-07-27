using Andor.Accounts.Application;
using Andor.Accounts.Binder;
using Andor.Accounts.Service.Consumers;
using Andor.Accounts.Service.WebSockets;
using Andor.Authentication.Jwt;
using Andor.Authorizations.Application;
using Andor.Documentation.Swagger;
using Andor.Foundation.Binder;
using Andor.Foundation.ServerServices;
using Andor.ServiceDefaults;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IWebSocketMessage, WebSocketMessages>();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.AddSwagger();

builder.AddFoundationCors();

builder.Services.ConfigureJwt(builder.Configuration);

builder.UseAkkaModules("AndorAccountsSystem");

builder.UseAccounts(builder.Configuration);

builder.Services.UseAuthorizations();

builder.Services.Configure<SignupVerifiedSubscriptionOptions>(
    builder.Configuration.GetSection(SignupVerifiedSubscriptionOptions.SectionName));

builder.Services.AddHostedService<UserVerifiedConsumer>();

builder.Services.Configure<AccountCreatedSubscriptionOptions>(
    builder.Configuration.GetSection(AccountCreatedSubscriptionOptions.SectionName));

builder.Services.AddHostedService<AccountCreatedConsumer>();

builder.Services.Configure<CashFlowProjectionSubscriptionOptions>(
    builder.Configuration.GetSection(CashFlowProjectionSubscriptionOptions.SectionName));

builder.Services.AddHostedService<CashFlowProjectionConsumer>();

var app = builder.Build();

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
                WebSocketMessages.WebSocketConnections.Add(new WebSocketMessages.WebSocketConnection(Guid.Parse(clientId),
                    Guid.Parse(sessionId),
                    webSocket));

                await Class.EchoWebSocket(Guid.Parse(clientId), Guid.Parse(sessionId), webSocket, WebSocketMessages.WebSocketConnections);
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

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseCustomSwagger(
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>(),
    app.Configuration);

app.UseHttpsRedirection();

app.UseFoundationCors();

app.MapDefaultEndpoints();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.ApplyAccountsMigrationsAsync();

await app.SeedDefaultCurrencyAsync();

app.Run();
