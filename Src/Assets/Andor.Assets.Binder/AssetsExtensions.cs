using Andor.Assets.Binder.Application;
using Andor.Assets.Binder.Infrastructure;
using Andor.Assets.RestApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Andor.Assets.Binder;

public static class AssetsExtensions
{
    public static WebApplicationBuilder UseAssets(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.UseApi()
            .WithAssetsApplication()
            .WithConfigurationInfrastructure(configuration);

        return builder;
    }

}
