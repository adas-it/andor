using Akka.Hosting;

namespace Andor.Foundation.Binder;

public interface IAkkaModule
{
    void Configure(AkkaConfigurationBuilder builder, IServiceProvider provider);
}
