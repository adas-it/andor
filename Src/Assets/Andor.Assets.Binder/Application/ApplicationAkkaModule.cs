using Akka.DependencyInjection;
using Akka.Hosting;
using Andor.Assets.Application.Actors;
using Andor.Foundation.Binder;

namespace Andor.Assets.Binder.Application;

internal class ApplicationAkkaModule : IAkkaModule
{
    public void Configure(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithActors((system, registry) =>
        {
            var props = DependencyResolver.For(system)
                .Props<AreaManagerActor>();

            var actorRef = system.ActorOf(props, nameof(AreaManagerActor));
            registry.Register<AreaManagerActor>(actorRef);
        });
    }
}
