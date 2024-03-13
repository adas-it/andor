using Andor.Domain.SeedWork;

namespace Andor.Domain.Entities.Users.DomainEvents;

public record UserCreatedDomainEvent(User user)
    : DomainEventBase<User>(nameof(UserCreatedDomainEvent), user);

public record UserUpdatedDomainEvent(User user)
    : DomainEventBase<User>(nameof(UserUpdatedDomainEvent), user);

public record UserDeletedDomainEvent(User user)
    : DomainEventBase<User>(nameof(UserDeletedDomainEvent), user);
