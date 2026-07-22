using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Andor.ComponentTests.Common;

public static class TestAuthHeaders
{
    public const string UserId = "X-Test-User-Id";
    public const string Group = "X-Test-Group";
    public const string Tenant = "X-Test-Tenant";
    public const string Anonymous = "X-Test-Anonymous";
}

/// <summary>
/// Fake authentication scheme used in component tests instead of real OpenIddict/JWT
/// validation. Reads the impersonated user from request headers (set via
/// <see cref="HttpClientTestExtensions.SetTestUser"/>) and builds the same "sub"/"role"/
/// "tenant_id" claims a real token would carry, so <c>ICurrentUserService</c> behaves
/// identically. Requests carrying <see cref="Anonymous"/> authenticate as "no result", which the
/// authorization middleware turns into a 401 challenge - exactly like a request with no token.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "ComponentTest";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.ContainsKey(TestAuthHeaders.Anonymous))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var id = Request.Headers.TryGetValue(TestAuthHeaders.UserId, out var userIdHeader)
            && Guid.TryParse(userIdHeader.ToString(), out var parsed)
                ? parsed
                : TestUser.Default.Id;

        var group = Request.Headers.TryGetValue(TestAuthHeaders.Group, out var groupHeader)
            ? groupHeader.ToString()
            : TestUser.Default.Group;

        var tenant = Request.Headers.TryGetValue(TestAuthHeaders.Tenant, out var tenantHeader)
            ? tenantHeader.ToString()
            : TestUser.Default.Tenant;

        var claims = new[]
        {
            new Claim("sub", id.ToString()),
            new Claim("role", group),
            new Claim("tenant_id", tenant),
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
