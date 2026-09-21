namespace Andor.Users.WebApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

[ApiController]
public class TokenController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<ApplicationUser> _hasher;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public TokenController(AppDbContext db, IPasswordHasher<ApplicationUser> hasher,
        IOpenIddictApplicationManager applicationManager)
    {
        _db = db;
        _hasher = hasher;
        _applicationManager = applicationManager;
    }

    [HttpPost("/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict server request not found.");

        if (request.IsPasswordGrantType())
            return await HandlePasswordFlow(request);

        if (request.IsAuthorizationCodeGrantType())
            return await HandleAuthorizationCodeFlow();

        if (request.IsClientCredentialsGrantType())
            return await HandleClientCredentialsFlow(request);

        return BadRequest(new { error = "unsupported_grant_type" });
    }

    private async Task<IActionResult> HandlePasswordFlow(OpenIddictRequest request)
    {
        var user = _db.Users.SingleOrDefault(u => u.UserName == request.Username);

        if (user is null)
            return BadRequest(new { error = "invalid_grant", error_description = "User not found." });

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return BadRequest(new { error = "invalid_grant", error_description = "Invalid password." });

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, user.Id.ToString());
        identity.AddClaim(Claims.Name, user.UserName!);
        identity.AddClaim(Claims.Role, user.Group);
        // Fixed to "TenantA" for every user until multi-tenant signup exists.
        identity.AddClaim("tenant_id", "TenantA");

        // Coarse-grained group claim only: resource APIs resolve fine-grained permissions
        // for the group server-side, so a permission change never requires re-issuing tokens.
        foreach (var claim in identity.Claims)
        {
            claim.SetDestinations(Destinations.AccessToken);
        }

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleAuthorizationCodeFlow()
    {
        // Retrieve the claims principal stored in the authorization code.
        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return BadRequest(new { error = "invalid_grant", error_description = "The authorization code is invalid." });

        var principal = result.Principal!;

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleClientCredentialsFlow(OpenIddictRequest request)
    {
        // The client itself was already authenticated (client_id + secret) by OpenIddict's own
        // token-endpoint pipeline before this action runs — same trust boundary the password flow
        // above relies on for authorization codes. This only needs to look the client up to grant
        // it the scopes it's actually permitted, not re-verify its identity.
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
            ?? throw new InvalidOperationException("The client application details cannot be found.");

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, request.ClientId!);
        identity.AddClaim(Claims.Name, (await _applicationManager.GetDisplayNameAsync(application)) ?? request.ClientId!);

        foreach (var claim in identity.Claims)
        {
            claim.SetDestinations(Destinations.AccessToken);
        }

        var principal = new ClaimsPrincipal(identity);

        // Only grant scopes this specific client was registered with
        // (Permissions.Prefixes.Scope + "…") — a client can't request its way into a scope nobody
        // gave it, regardless of what it asks for.
        var grantedScopes = new List<string>();

        foreach (var scope in request.GetScopes())
        {
            if (await _applicationManager.HasPermissionAsync(application, Permissions.Prefixes.Scope + scope))
            {
                grantedScopes.Add(scope);
            }
        }

        principal.SetScopes(grantedScopes);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
