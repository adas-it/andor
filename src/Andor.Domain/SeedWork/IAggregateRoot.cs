namespace Andor.Domain.SeedWork;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEventBase> Events { get; }

    void ClearEvents();
}
