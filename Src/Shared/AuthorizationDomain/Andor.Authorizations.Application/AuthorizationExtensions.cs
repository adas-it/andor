using Andor.Authorizations.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Authorizations.Application;

public static class AuthorizationExtensions
{
    public static IServiceCollection UseAuthorizations(this IServiceCollection services)
    {
        return services.AddHttpContextAccessor()
            .AddScoped<AuthorizationDomain>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<IAuthorizationService, AuthorizationService>()
            .AddSingleton<IAuthorizationRepository, AuthorizationRepository>()
            .AddSingleton<IUserContextAccessor, UserContextAccessor>();
    }
}
