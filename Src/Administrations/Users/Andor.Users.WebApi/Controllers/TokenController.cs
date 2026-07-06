namespace Andor.Users.WebApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore;
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
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is null || request.GrantType != GrantTypes.Password)
            return BadRequest(new { error = "unsupported_grant_type" });

        var user = _db.Users.SingleOrDefault(u => u.UserName == request.Username);

        if (user == null)
            return BadRequest(new { error = "invalid_grant", error_description = "User not found" });

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return BadRequest(new { error = "invalid_grant", error_description = "Invalid password" });

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, user.Id.ToString());
        identity.AddClaim(Claims.Name, user.UserName);

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
