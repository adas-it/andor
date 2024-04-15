using Andor.Domain.Onboarding.Users;

namespace Andor.Domain.Onboarding.Users.DomainEvents;

public record UserCreatedDomainEvent
{
    public Guid Id { get; init; }
    public static UserCreatedDomainEvent FromAggregateRoot(User entity)
        => new UserCreatedDomainEvent() with
        {
            Id = entity.Id
        };
}

public record UserUpdatedDomainEvent
{
    public Guid Id { get; init; }
    public static UserUpdatedDomainEvent FromAggregateRoot(User entity)
        => new UserUpdatedDomainEvent() with
        {
            Id = entity.Id
        };
}

public record UserDeletedDomainEvent
{
    public Guid Id { get; init; }
    public static UserDeletedDomainEvent FromAggregateRoot(User entity)
        => new UserDeletedDomainEvent() with
        {
            Id = entity.Id
        };
}
