using Andor.Authorizations.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Authorizations.Application;

public static class AuthorizationExtensions
{
    public static IServiceCollection UseAuthorizations(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationDomain>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddSingleton<IAuthorizationRepository, AuthorizationRepository>();

        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

        return services;
    }
}
