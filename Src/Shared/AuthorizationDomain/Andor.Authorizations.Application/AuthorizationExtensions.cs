using Andor.Authorizations.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Authorizations.Application;

public static class AuthorizationExtensions
{
    public static IServiceCollection UseAuthorizations(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

        return services;
    }
}
