using Andor.Foundation.Domain.Events;

namespace Andor.Foundation.Domain.SeedWork;

public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEvent> Events { get; }

    void ClearEvents();
}
