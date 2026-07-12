using Akka.DependencyInjection;
using Akka.Hosting;
using Andor.Configurations.Application.Actors;
using Andor.Foundation.Binder;

namespace Andor.Configurations.Binder.Application;

internal class ApplicationAkkaModule : IAkkaModule
{
    public void Configure(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithActors((system, registry) =>
        {
            var props = DependencyResolver.For(system)
                            .Props<ConfigurationManagerActor>();

            var actorRef = system.ActorOf(props, nameof(ConfigurationManagerActor));
            registry.Register<ConfigurationManagerActor>(actorRef);
        });
    }
}
