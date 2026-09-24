using Andor.Authentication.Jwt;
using Andor.Authorizations.Application;
using Andor.Documentation.Swagger;
using Andor.Foundation.ServerServices;
using Andor.ServiceDefaults;
using Andor.Users.Binder;
using Andor.Users.Service.Consumers;
using Asp.Versioning.ApiExplorer;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.AddSwagger();

builder.AddFoundationCors();

builder.Services.ConfigureJwt(builder.Configuration);

builder.UseUsers(builder.Configuration);
builder.Services.UseAuthorizations();

// Lets Onboarding's client-credentials token call POST /v1/users; nothing else needs this scope.
builder.Services.AddAuthorization();

builder.Services.AddOptions<UserProvisioningQueueOptions>()
    .Bind(builder.Configuration.GetSection(UserProvisioningQueueOptions.SectionName));

// Keyed, separate from the shared ServiceBusClient this app publishes its own events through:
// this queue is meant to carry a Listen-only SAS scoped to just "request-user-provisioning".
builder.Services.AddKeyedSingleton(UserProvisioningRequestedConsumer.ClientKey, (serviceProvider, _) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<UserProvisioningQueueOptions>>().Value;

    var clientOptions = new ServiceBusClientOptions
    {
        TransportType = ServiceBusTransportType.AmqpWebSockets,
    };

    if (!string.IsNullOrWhiteSpace(options.ConnectionString))
    {
        return new ServiceBusClient(options.ConnectionString, clientOptions);
    }

    var credentialOptions = new DefaultAzureCredentialOptions();

    if (builder.Environment.IsDevelopment())
    {
        credentialOptions.ExcludeManagedIdentityCredential = true;
        credentialOptions.ExcludeWorkloadIdentityCredential = true;
    }

    return new ServiceBusClient(
        options.FullyQualifiedNamespace,
        new DefaultAzureCredential(credentialOptions),
        clientOptions);
});

builder.Services.AddHostedService<UserProvisioningRequestedConsumer>();

var app = builder.Build();

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

await app.ApplyUserMigrationsAsync();

app.Run();
