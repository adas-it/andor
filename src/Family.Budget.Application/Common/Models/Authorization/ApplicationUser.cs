using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Family.Budget.Application.Models.Authorization
{
    public class ApplicationUser
    {
        public ApplicationUser(IHttpContextAccessor httpContext)
        {
            IsAuthenticated = httpContext?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            UserId = Guid.Parse(httpContext?.HttpContext?.User?.FindFirstValue(ApplicationClaims.Id) ?? "dee240d6-39a1-423b-ac31-10c991759cdd");
            Name = httpContext?.HttpContext?.User?.FindFirstValue(ApplicationClaims.Name) ?? "system";
            UserClaims = httpContext?.HttpContext?.User.Claims ?? new HashSet<Claim>();
        }

        public Guid UserId { get; }
        public string Name { get; }
        public bool IsAuthenticated { get; }
        public IEnumerable<Claim> UserClaims { get; }
    }
}
