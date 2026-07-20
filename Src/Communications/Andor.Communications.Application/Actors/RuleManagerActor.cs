using Akka.Actor;
using Andor.Communications.Application.Commands;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;

namespace Andor.Communications.Application.Actors;

public class RuleManagerActor : ReceiveActor
{
    public RuleManagerActor(IServiceProvider serviceProvider)
    {
        Receive<CreateRuleCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });

        Receive<SendNotificationCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });
    }

    private static void Handler(IServiceProvider serviceProvider, ICommands<RuleId> cmd)
    {
        var childName = $"{nameof(RuleActor)}-{cmd.Id.ToString()}";

        var child = Context.Child(childName);

        if (child.Equals(ActorRefs.Nobody))
        {
            child = Context.ActorOf(Props.Create(
                () => new RuleActor(cmd.Id, serviceProvider)), childName);
        }

        child.Forward(cmd);
    }
}
