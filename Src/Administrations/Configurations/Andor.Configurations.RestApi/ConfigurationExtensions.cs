using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Configurations.RestApi;

public static class ApiExtensions
{
    public static IServiceCollection UseApi(this IServiceCollection services)
    {
        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ConfigurationsController).Assembly));

        return services;
    }
}
