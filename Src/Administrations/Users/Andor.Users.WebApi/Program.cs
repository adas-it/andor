using Andor.Users.WebApi;
using Andor.Users.WebApi.Consumers;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Kestrel sits behind Azure's ingress, which terminates TLS and forwards plain HTTP.
// Without this, OpenIddict sees every request as HTTP and rejects it (ID2083).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

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
        _ = opt.AllowRefreshTokenFlow();
        _ = opt.AllowClientCredentialsFlow();

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

builder.Services.AddOptions<IdentityProvisioningQueueOptions>()
    .Bind(builder.Configuration.GetSection(IdentityProvisioningQueueOptions.SectionName));

builder.Services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<IdentityProvisioningQueueOptions>>().Value;

    var clientOptions = new ServiceBusClientOptions
    {
        TransportType = ServiceBusTransportType.AmqpWebSockets,
    };

    // An explicit connection string takes precedence (e.g. local dev via User Secrets). This is a
    // Listen-only SAS scoped to just this one queue — not the app's broader ServiceBus credential.
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

builder.Services.AddHostedService<IdentityProvisioningConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //_ = await context.Database.EnsureCreatedAsync();

    await context.Database.MigrateAsync();


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

    // Service-to-service client: lets Onboarding call Users.Service's POST /users and
    // Communications.Service's POST /communications/requests with a client-credentials token
    // instead of those endpoints trusting Service Bus alone. The secret MUST be overridden
    // outside appsettings.json for any non-local environment (User Secrets / Key Vault / env
    // var) — this fallback only exists so local dev works out of the box. Upserted (not just
    // created-once) so adding a new scope here actually reaches an already-seeded client on
    // redeploy, the same way EnsureSpaClientAsync does for the SPA clients below.
    var onboardingServiceConfig = app.Configuration.GetSection("OpenIddictClients:OnboardingService");
    var onboardingServiceSecret = onboardingServiceConfig["ClientSecret"] ?? "dev-only-onboarding-service-secret";
    var onboardingServiceClientId = onboardingServiceConfig["ClientId"] ?? "onboarding-service";

    var existingOnboardingServiceClient = await manager.FindByClientIdAsync(onboardingServiceClientId);

    var onboardingServiceDescriptor = new OpenIddictApplicationDescriptor
    {
        ClientId = onboardingServiceClientId,
        ClientSecret = onboardingServiceSecret,
        DisplayName = "Onboarding Service",
        Permissions =
        {
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
            OpenIddictConstants.Permissions.Prefixes.Scope + "users.write",
            OpenIddictConstants.Permissions.Prefixes.Scope + "communications.write"
        }
    };

    if (existingOnboardingServiceClient is null)
    {
        _ = await manager.CreateAsync(onboardingServiceDescriptor);
    }
    else
    {
        await manager.UpdateAsync(existingOnboardingServiceClient, onboardingServiceDescriptor);
    }

    var webAppConfig = app.Configuration.GetSection("OpenIddictClients:WebApp");
    await EnsureSpaClientAsync(
        manager,
        clientId: webAppConfig["ClientId"] ?? "web-app",
        displayName: webAppConfig["DisplayName"] ?? "Web Application",
        redirectUris: webAppConfig.GetSection("RedirectUris").Get<string[]>() ?? [],
        permissions:
        [
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile
        ]);

    var kenobiConfig = app.Configuration.GetSection("OpenIddictClients:Kenobi");
    await EnsureSpaClientAsync(
        manager,
        clientId: kenobiConfig["ClientId"] ?? "kenobi",
        displayName: kenobiConfig["DisplayName"] ?? "Kenobi SPA",
        redirectUris: kenobiConfig.GetSection("RedirectUris").Get<string[]>() ?? [],
        permissions:
        [
            OpenIddictConstants.Permissions.GrantTypes.Password,
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess
        ]);
}

static async Task EnsureSpaClientAsync(
    IOpenIddictApplicationManager manager,
    string clientId,
    string displayName,
    IReadOnlyCollection<string> redirectUris,
    IReadOnlyCollection<string> permissions)
{
    var existingClient = await manager.FindByClientIdAsync(clientId);

    var descriptor = new OpenIddictApplicationDescriptor();
    if (existingClient is not null)
        await manager.PopulateAsync(descriptor, existingClient);

    descriptor.ClientId = clientId;
    descriptor.DisplayName = displayName;

    descriptor.Permissions.Clear();
    foreach (var permission in permissions)
        _ = descriptor.Permissions.Add(permission);

    descriptor.RedirectUris.Clear();
    foreach (var uri in redirectUris)
        _ = descriptor.RedirectUris.Add(new Uri(uri));

    if (existingClient is null)
        _ = await manager.CreateAsync(descriptor);
    else
        await manager.UpdateAsync(existingClient, descriptor);
}

if (app.Environment.IsDevelopment())
{
    var clientId = app.Configuration.GetSection("OpenIddictClients:WebApp")["ClientId"] ?? "web-app";

    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Andor Users API v1");
        o.OAuthClientId(clientId);
        o.OAuthAppName("Andor Users - Swagger");
        o.OAuthUsePkce();
    });
}

app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSwaggerOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
