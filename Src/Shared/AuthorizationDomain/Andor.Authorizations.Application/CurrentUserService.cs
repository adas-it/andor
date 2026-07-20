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
                var userIdClaim = user.FindFirst("sub")?.Value
                                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var userId = Guid.TryParse(userIdClaim, out var parsedId)
                    ? parsedId
                    : Guid.NewGuid();

                var tenantId = user.FindFirst("tenant_id")?.Value
                               ?? user.FindFirst("organization")?.Value
                               ?? "TenantA";

                // "role" is the short claim name OpenIddict issues (see Andor.Users.WebApi's
                // TokenController); ClaimTypes.Role is populated instead when the principal comes
                // from an external IdP via ClaimsTransformer. Check both so either path works.
                var groupName = user.FindFirst("role")?.Value
                                 ?? user.FindFirst(ClaimTypes.Role)?.Value
                                 ?? "User";

                return new ApplicationUser(userId, groupName, isAuthenticated, tenantId);
            }

            return new ApplicationUser(Guid.NewGuid(), "Anonymous", false, "TenantA");
        }

        return new ApplicationUser(Guid.Empty, "System", false, "TenantA");
    }
}
