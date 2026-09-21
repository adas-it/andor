using Andor.Authentication.Jwt;
using Andor.Authorizations.Application;
using Andor.Communications.Binder;
using Andor.Communications.Service.Consumers;
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

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.AddSwagger();

builder.AddFoundationCors();

builder.Services.ConfigureJwt(builder.Configuration);

builder.UseAkkaModules("AndorCommunicationsSystem");

builder.UseCommunications(builder.Configuration);

builder.Services.UseAuthorizations();

// Lets Onboarding's client-credentials token call POST /v1/communications/requests.
builder.Services.AddAuthorization(options =>
    options.AddPolicy("communications.write", policy => policy.Requirements.Add(new ScopeRequirement("communications.write"))));

builder.Services.AddOptions<RecipientSyncSubscriptionOptions>()
    .Bind(builder.Configuration.GetSection(RecipientSyncSubscriptionOptions.SectionName));

builder.Services.AddHostedService<RecipientSyncConsumer>();

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

await app.ApplyCommunicationMigrationsAsync();

app.Run();
