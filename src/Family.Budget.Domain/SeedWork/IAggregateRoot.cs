﻿namespace Family.Budget.Domain.SeedWork;

public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEventBase> Events { get; }

    void ClearEvents();
}
