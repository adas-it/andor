using Akka.Actor;
using Andor.Foundation.Application.Commands;
using Andor.Onboarding.Application.Commands;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Application.Actors;

public class SignupManagerActor : ReceiveActor
{
    public SignupManagerActor(IServiceProvider serviceProvider)
    {
        Receive<StartSignupCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });

        Receive<VerifySignupCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });
    }

    private static void Handler(IServiceProvider serviceProvider, ICommands<SignupRequestId> cmd)
    {
        var childName = $"{nameof(SignupActor)}-{cmd.Id.ToString()}";

        var child = Context.Child(childName);

        if (child.Equals(ActorRefs.Nobody))
        {
            child = Context.ActorOf(Props.Create(
                () => new SignupActor(cmd.Id, serviceProvider)), childName);
        }

        child.Forward(cmd);
    }
}
