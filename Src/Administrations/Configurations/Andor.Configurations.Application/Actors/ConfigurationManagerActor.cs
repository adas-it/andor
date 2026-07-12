using Akka.Actor;
using Andor.Configurations.Application.Commands;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;

namespace Andor.Configurations.Application.Actors;

public class ConfigurationManagerActor : ReceiveActor
{
    public ConfigurationManagerActor(IServiceProvider serviceProvider)
    {
        Receive<DeactivateConfigurationCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });

        Receive<ChangeConfigurationCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });

        Receive<CreateConfigurationCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });

        Receive<DeleteConfigurationCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });
    }

    private static void Handler(IServiceProvider serviceProvider, ICommands<ConfigurationId> cmd)
    {
        var childName = $"{nameof(ConfigurationActor)}-{cmd.Id.ToString()}";

        var child = Context.Child(childName);

        if (child.Equals(ActorRefs.Nobody))
        {
            child = Context.ActorOf(Props.Create(
                () => new ConfigurationActor(cmd.Id, serviceProvider)), childName);
        }

        child.Forward(cmd);
    }
}
