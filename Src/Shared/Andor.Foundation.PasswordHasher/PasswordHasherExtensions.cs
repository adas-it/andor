using Microsoft.Extensions.DependencyInjection;

namespace Andor.Foundation.PasswordHasher;

public static class PasswordHasherExtensions
{
    public static IServiceCollection WithPasswordHasher(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }
}

