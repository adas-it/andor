namespace Andor.Users.WebApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static OpenIddict.Abstractions.OpenIddictConstants;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<ApplicationUser> _hasher;

    public AccountController(AppDbContext db, IPasswordHasher<ApplicationUser> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [HttpGet("/account/login")]
    public IActionResult Login(string? returnUrl)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("/account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _db.Users.SingleOrDefault(u => u.UserName == model.UserName);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var verification = _hasher.VerifyHashedPassword(user, user.PasswordHash!, model.Password!);

        if (verification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        // Sign in with a cookie so ConnectController can detect the session.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(new Claim(Claims.Subject, user.Id.ToString()));
        identity.AddClaim(new Claim(Claims.Name, user.UserName!));

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("Cookie", principal);

        // Redirect back to the original /connect/authorize?... URL.
        var returnUrl = model.ReturnUrl;
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return Redirect("/");
    }
}

public class LoginViewModel
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ReturnUrl { get; set; }
}
