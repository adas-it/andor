using Akka.DependencyInjection;
using Akka.Hosting;
using Andor.Accounts.Application.Actors;
using Andor.Foundation.Binder;

namespace Andor.Accounts.Binder.Application;

internal class ApplicationAkkaModule : IAkkaModule
{
    public void Configure(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithActors((system, registry) =>
        {
            var props = DependencyResolver.For(system)
                            .Props<AccountManagerActor>();

            var actorRef = system.ActorOf(props, nameof(AccountManagerActor));
            registry.Register<AccountManagerActor>(actorRef);
        });
    }
}
