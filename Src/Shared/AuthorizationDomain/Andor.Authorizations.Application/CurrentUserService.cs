using System.Security.Claims;
using Andor.Authorizations.Domain;
using Microsoft.AspNetCore.Http;

namespace Andor.Authorizations.Application;

internal class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    IUserContextAccessor userContextAccessor) : ICurrentUserService
{
    public ApplicationUser GetCurrentUser()
    {
        if (userContextAccessor.CurrentUser is not null)
        {
            return userContextAccessor.CurrentUser;
        }

        if (httpContextAccessor?.HttpContext is not null)
        {
            var user = httpContextAccessor.HttpContext.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                var userName = user.FindFirst("preferred_username")?.Value
                               ?? user.FindFirst(ClaimTypes.Name)?.Value
                               ?? user?.Identity?.Name
                               ?? "Anonymous";

                var userIdClaim = user.FindFirst("sub")?.Value
                                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var userId = Guid.TryParse(userIdClaim, out var parsedId)
                    ? parsedId
                    : Guid.NewGuid();

                var tenantId = user.FindFirst("tenant_id")?.Value
                               ?? user.FindFirst("organization")?.Value
                               ?? "TenantA";

                return new ApplicationUser(userId, userName, isAuthenticated, tenantId);
            }

            return new ApplicationUser(Guid.NewGuid(), "Anonymous", false, "TenantA");
        }

        return new ApplicationUser(Guid.Empty, "System", false, "TenantA");
    }
}
