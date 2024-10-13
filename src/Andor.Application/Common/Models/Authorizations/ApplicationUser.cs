using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Andor.Application.Common.Models.Authorizations;

public class ApplicationUser
{
    public ApplicationUser(HttpContext httpContext)
    {
        UserId = UserId.Load(httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "dee240d6-39a1-423b-ac31-10c991759cdd");
        Name = httpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "system";
        IsAuthenticated = httpContext?.User?.Identity?.IsAuthenticated ?? false;
        UserClaims = httpContext?.User.Claims.ToList() ?? [];
    }

    public UserId UserId { get; }
    public string Name { get; }
    public bool IsAuthenticated { get; }
    public List<Claim> UserClaims { get; }
}
