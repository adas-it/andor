namespace Andor.Users.WebApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

public class ConnectController : Controller
{
    [HttpGet("/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        _ = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict server request not found.");

        // Check if the user is already signed in via the login cookie.
        var result = await HttpContext.AuthenticateAsync("Cookie");
        if (!result.Succeeded)
        {
            // Not authenticated — redirect to the login page, preserving the full authorize query string.
            return RedirectToAction("Login", "Account",
                new { returnUrl = Request.PathBase + Request.Path + Request.QueryString });
        }

        // User is authenticated — issue the authorization code.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        foreach (var claim in result.Principal!.Claims)
            identity.AddClaim(claim);

        var principal = new ClaimsPrincipal(identity);

        // Forward all requested scopes.
        var request = HttpContext.GetOpenIddictServerRequest()!;
        principal.SetScopes(request.GetScopes());

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
