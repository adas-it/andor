using Akka.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Foundation.Binder;

public static class AkkaModuleStarter
{
    public static WebApplicationBuilder UseAkkaModules(this WebApplicationBuilder builder, string actorSystemName)
    {
        builder.Services.AddAkka(actorSystemName, (akkaConfigurationBuilder, provider) =>
        {
            var modules = provider.GetServices<IAkkaModule>();

            foreach (var module in modules)
            {
                module.Configure(akkaConfigurationBuilder, provider);
            }
        });

        return builder;
    }
}