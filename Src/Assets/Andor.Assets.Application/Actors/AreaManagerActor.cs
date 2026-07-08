using Akka.Actor;
using Andor.Assets.Application.Commands;
using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Foundation.Application.Commands;

namespace Andor.Assets.Application.Actors;

public class AreaManagerActor : ReceiveActor
{
    public AreaManagerActor(IServiceProvider serviceProvider)
    {
        Receive<CreateAreaCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });
    }

    private static void Handler(IServiceProvider serviceProvider, ICommands<AreaId> cmd)
    {
        var childName = $"{nameof(AreaActor)}-{cmd.Id.ToString()}";

        var child = Context.Child(childName);

        if (child.Equals(ActorRefs.Nobody))
        {
            child = Context.ActorOf(Props.Create(
                () => new AreaActor(cmd.Id, serviceProvider)), childName);
        }

        child.Forward(cmd);
    }
}

