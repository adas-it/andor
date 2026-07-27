using Andor.Accounts.Application.Commands;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Application.Queries;
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

        _ = services.AddScoped<IAccountSubCategoriesQueriesService, AccountSubCategoriesQueriesService>();

        _ = services.AddScoped<IAccountCategoriesQueriesService, AccountCategoriesQueriesService>();

        _ = services.AddScoped<IAccountPaymentMethodQueriesService, AccountPaymentMethodQueriesService>();

        _ = services.AddScoped<IAccountFinancialMovementsQueriesService, AccountFinancialMovementsQueriesService>();

        return services;
    }
}
