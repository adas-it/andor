using System.Security.Claims;

namespace Family.Budget.Application.Models.Authorization
{
    public static class ApplicationClaims
    {
        public static readonly string Id = ClaimTypes.NameIdentifier;
        public static readonly string Name = ClaimTypes.Name;
        public static readonly string Role = ClaimTypes.Role;
    }
}
