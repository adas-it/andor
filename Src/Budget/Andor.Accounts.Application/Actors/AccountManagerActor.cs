using Akka.Actor;
using Andor.Accounts.Application.Commands;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Application.Commands;

namespace Andor.Accounts.Application.Actors;

public class AccountManagerActor : ReceiveActor
{
    public AccountManagerActor(IServiceProvider serviceProvider)
    {
        Receive<CreateAccountCommand>(cmd =>
        {
            Handler(serviceProvider, cmd);
        });
    }

    private static void Handler(IServiceProvider serviceProvider, ICommands<AccountId> cmd)
    {
        var childName = $"{nameof(AccountActor)}-{cmd.Id.ToString()}";

        var child = Context.Child(childName);

        if (child.Equals(ActorRefs.Nobody))
        {
            child = Context.ActorOf(Props.Create(
                () => new AccountActor(cmd.Id, serviceProvider)), childName);
        }

        child.Forward(cmd);
    }
}
