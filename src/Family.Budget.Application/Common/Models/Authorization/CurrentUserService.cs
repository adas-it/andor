using Microsoft.AspNetCore.Http;

namespace Family.Budget.Application.Models.Authorization
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContext)
        {
            User = new ApplicationUser(httpContext);
        }

        public ApplicationUser User { get; }
    }
}
