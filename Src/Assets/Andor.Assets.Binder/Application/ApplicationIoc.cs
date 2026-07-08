using Andor.Assets.Application;
using Andor.Assets.Application.Interfaces;
using Andor.Assets.Domain.Investments.Tickers;
using Andor.Foundation.Binder;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Assets.Binder.Application;

internal static class ApplicationIoc
{
    public static IServiceCollection WithAssetsApplication(this IServiceCollection services)
    {
        services.AddSingleton<IAkkaModule, ApplicationAkkaModule>();

        services.AddScoped<ITickerValidator, TickerValidator>();

        services.AddScoped<IAreaCommandsService, AreaCommandsService>();

        services.AddScoped<IAreaQueriesService, AreaQueriesService>();

        return services;
    }
}
