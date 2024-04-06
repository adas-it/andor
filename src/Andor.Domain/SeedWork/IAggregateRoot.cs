namespace Andor.Domain.SeedWork;

public interface IAggregateRoot
{
    IReadOnlyCollection<object> Events { get; }

    void ClearEvents();
}
