using Andor.Users.Application;
using Andor.Users.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Andor.Users.Domain.Users;

namespace Andor.Users.Binder.Application;

internal static class ApplicationIoc
{
    public static IServiceCollection WithUserApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserValidator, UserValidator>();

        services.AddScoped<IUserQueriesService, UserQueriesService>();

        services.AddScoped<IUserCommandsService, UserCommandsService>();

        return services;
    }
}
