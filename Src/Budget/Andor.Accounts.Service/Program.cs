using Andor.Accounts.Binder;
using Andor.Accounts.Service.Consumers;
using Andor.Authentication.Jwt;
using Andor.Authorizations.Application;
using Andor.Documentation.Swagger;
using Andor.Foundation.Binder;
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

builder.Services.ConfigureJwt(builder.Configuration);

builder.UseAkkaModules("AndorAccountsSystem");

builder.UseAccounts(builder.Configuration);

builder.Services.UseAuthorizations();

builder.Services.Configure<SignupVerifiedSubscriptionOptions>(
    builder.Configuration.GetSection(SignupVerifiedSubscriptionOptions.SectionName));

builder.Services.AddHostedService<UserVerifiedConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseCustomSwagger(
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>(),
    app.Configuration);

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.ApplyAccountsMigrationsAsync();

await app.SeedDefaultCurrencyAsync();

app.Run();
