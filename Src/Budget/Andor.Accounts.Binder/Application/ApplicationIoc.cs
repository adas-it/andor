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
        _ = services.AddSingleton<IAkkaModule, ApplicationAkkaModule>();

        _ = services.AddScoped<IAccountValidator, AccountValidator>();

        _ = services.AddScoped<IAccountCommandsService, AccountCommandsService>();

        _ = services.AddScoped<IAccountQueriesService, AccountQueriesService>();

        return services;
    }
}
