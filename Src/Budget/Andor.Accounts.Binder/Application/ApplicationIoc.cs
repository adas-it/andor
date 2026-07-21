using Andor.Accounts.Application;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Domain.Accounts;
using Andor.Foundation.Binder;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.Binder.Application;

internal static class ApplicationIoc
{
    public static IServiceCollection WithAccountsApplication(this IServiceCollection services)
    {
        services.AddSingleton<IAkkaModule, ApplicationAkkaModule>();

        services.AddScoped<IAccountValidator, AccountValidator>();

        services.AddScoped<IAccountCommandsService, AccountCommandsService>();

        return services;
    }
}
