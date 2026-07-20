using Akka.DependencyInjection;
using Akka.Hosting;
using Andor.Communications.Application.Actors;
using Andor.Foundation.Binder;

namespace Andor.Communications.Binder.Application;

internal class ApplicationAkkaModule : IAkkaModule
{
    public void Configure(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithActors((system, registry) =>
        {
            var props = DependencyResolver.For(system)
                            .Props<RuleManagerActor>();

            var actorRef = system.ActorOf(props, nameof(RuleManagerActor));
            registry.Register<RuleManagerActor>(actorRef);
        });
    }
}
