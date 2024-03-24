using Andor.Domain.SeedWork;

namespace Andor.Domain.Entities.Onboarding.Registrations.DomainEvents;

public sealed record RegistrationCreatedDomainEvent(Registration registration)
    : DomainEventBase<Registration>(nameof(RegistrationCreatedDomainEvent), registration);

public sealed record RegistrationCodeChangedDomainEvent(Registration registration)
    : DomainEventBase<Registration>(nameof(RegistrationCreatedDomainEvent), registration);
