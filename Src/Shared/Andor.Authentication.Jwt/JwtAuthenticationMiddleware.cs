using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace Andor.Authentication.Jwt;

public static class JwtAuthenticationMiddleware
{
    public static IServiceCollection ConfigureJwt(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authOptions = configuration
            .GetSection(nameof(IdentityProvider))
            .Get<IdentityProvider>();

        if (string.IsNullOrWhiteSpace(authOptions?.Authority))
            return services;

        services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                // Resolve tokens issued by the OpenIddict server hosted in Andor.Users.WebApi.
                options.SetIssuer(new Uri(authOptions.Authority!));

                // Retrieve the server metadata and signing keys over HTTP.
                options.UseSystemNetHttp();

                // Register the ASP.NET Core host and the authentication handler.
                options.UseAspNetCore();
            });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            o.DefaultPolicy = new AuthorizationPolicyBuilder(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
