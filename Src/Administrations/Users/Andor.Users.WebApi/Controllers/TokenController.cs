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

    public TokenController(AppDbContext db, IPasswordHasher<ApplicationUser> hasher)
    {
        _db = db;
        _hasher = hasher;
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
}
