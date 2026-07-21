using Andor.Users.WebApi;
using Andor.Users.WebApi.Consumers;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Andor Users API", Version = "v1" });

    var authority = builder.Configuration["OpenIddict:Authority"] ?? "https://localhost:7116";
    var clientId = builder.Configuration.GetSection("OpenIddictClients:WebApp")["ClientId"] ?? "web-app";

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{authority}/connect/authorize"),
                TokenUrl = new Uri($"{authority}/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" }
                }
            }
        }
    });

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("oauth2")] = ["openid", "profile", "email"]
    });
});

builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    _ = options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    _ = options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        _ = opt.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(opt =>
    {
        _ = opt.AllowPasswordFlow();
        _ = opt.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        _ = opt.SetAuthorizationEndpointUris("/connect/authorize");
        _ = opt.SetTokenEndpointUris("/connect/token");

        _ = opt.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        _ = opt.DisableAccessTokenEncryption();

        _ = opt.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough();
    })
    .AddValidation(opt =>
    {
        _ = opt.UseLocalServer();
        _ = opt.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookie";
    options.DefaultChallengeScheme = "Cookie";
})
.AddCookie("Cookie", options =>
{
    options.LoginPath = "/account/login";
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("CorsAllowedOrigins").Get<string[]>() ?? [];
    options.AddPolicy("AllowSwaggerOrigins", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddOptions<UserVerifiedSubscriptionOptions>()
    .Bind(builder.Configuration.GetSection(UserVerifiedSubscriptionOptions.SectionName));

builder.Services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<UserVerifiedSubscriptionOptions>>().Value;

    var clientOptions = new ServiceBusClientOptions
    {
        TransportType = ServiceBusTransportType.AmqpWebSockets,
    };

    // An explicit connection string takes precedence (e.g. local dev via User Secrets).
    if (!string.IsNullOrWhiteSpace(options.ConnectionString))
    {
        return new ServiceBusClient(options.ConnectionString, clientOptions);
    }

    var credentialOptions = new DefaultAzureCredentialOptions();

    // Locally there is no IMDS endpoint, so the ManagedIdentity probe hangs and fails.
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

builder.Services.AddHostedService<UserVerifiedConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    _ = await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await manager.FindByClientIdAsync("console") is null)
    {
        _ = await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "console",
            ClientSecret = "secret",
            DisplayName = "Console Client",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password
            }
        });
    }

    var webAppConfig = app.Configuration.GetSection("OpenIddictClients:WebApp");
    var webAppClientId = webAppConfig["ClientId"] ?? "web-app";
    var webAppRedirectUris = webAppConfig.GetSection("RedirectUris").Get<string[]>() ?? [];

    var existingWebApp = await manager.FindByClientIdAsync(webAppClientId);
    if (existingWebApp is null)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = webAppClientId,
            DisplayName = webAppConfig["DisplayName"] ?? "Web Application",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile
            }
        };

        foreach (var uri in webAppRedirectUris)
            descriptor.RedirectUris.Add(new Uri(uri));

        _ = await manager.CreateAsync(descriptor);
    }
    else
    {
        var descriptor = new OpenIddictApplicationDescriptor();
        await manager.PopulateAsync(descriptor, existingWebApp);

        descriptor.RedirectUris.Clear();
        foreach (var uri in webAppRedirectUris)
            descriptor.RedirectUris.Add(new Uri(uri));

        await manager.UpdateAsync(existingWebApp, descriptor);
    }
}

if (app.Environment.IsDevelopment())
{
    var clientId = app.Configuration.GetSection("OpenIddictClients:WebApp")["ClientId"] ?? "web-app";

    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Andor Users API v1");
        o.OAuthClientId(clientId);
        o.OAuthAppName("Andor Users - Swagger");
        o.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSwaggerOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
