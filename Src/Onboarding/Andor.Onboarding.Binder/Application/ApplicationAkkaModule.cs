using Akka.DependencyInjection;
using Akka.Hosting;
using Andor.Foundation.Binder;
using Andor.Onboarding.Application.Actors;

namespace Andor.Onboarding.Binder.Application;

internal class ApplicationAkkaModule : IAkkaModule
{
    public void Configure(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithActors((system, registry) =>
        {
            var props = DependencyResolver.For(system)
                            .Props<SignupManagerActor>();

            var actorRef = system.ActorOf(props, nameof(SignupManagerActor));
            registry.Register<SignupManagerActor>(actorRef);
        });
    }
}
