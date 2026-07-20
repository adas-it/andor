using Andor.Authentication.Jwt;
using Andor.Authorizations.Application;
using Andor.Communications.Binder;
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

builder.UseAkkaModules("AndorCommunicationsSystem");

builder.UseCommunications(builder.Configuration);

builder.Services.UseAuthorizations();

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

app.Run();
