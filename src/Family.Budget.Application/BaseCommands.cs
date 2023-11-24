namespace Family.Budget.Application;
using Family.Budget.Application.Models;

public abstract class BaseCommands
{
    public Notifier _notifier { get; private set; }
    public BaseCommands(Notifier notifier)
    {
        _notifier = notifier;
    }
}
