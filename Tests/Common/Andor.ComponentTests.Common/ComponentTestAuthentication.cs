using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.ComponentTests.Common;

public static class ComponentTestAuthentication
{
    /// <summary>
    /// Registers <see cref="TestAuthHandler"/> as an additional scheme and repoints the
    /// authorization default/fallback policies at it via <c>PostConfigure</c>, which always runs
    /// after the module's own <c>ConfigureJwt</c> call regardless of registration order. Every
    /// plain <c>[Authorize]</c> in the app (no explicit scheme) ends up authenticating against
    /// this handler instead of the real OpenIddict validation handler.
    /// </summary>
    public static IServiceCollection AddComponentTestAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

        services.PostConfigure<AuthorizationOptions>(options =>
        {
            var policy = new AuthorizationPolicyBuilder(TestAuthHandler.SchemeName)
                .RequireAuthenticatedUser()
                .Build();

            options.DefaultPolicy = policy;
            options.FallbackPolicy = policy;
        });

        return services;
    }
}
