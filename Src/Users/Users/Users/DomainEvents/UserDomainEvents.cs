using Andor.Foundation.Domain.Events;

namespace Users.Users.DomainEvents;

public record UserCreatedDomainEvent : DomainEvent
{
    public Guid Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid PreferredCurrencyId { get; set; }
    public Guid PreferredLanguageId { get; set; }

    public static UserCreatedDomainEvent FromAggregateRoot(User entity)
        => new UserCreatedDomainEvent() with
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Address,
            PreferredCurrencyId = (Guid)entity.PreferredCurrencyId,
            PreferredLanguageId = (Guid)entity.PreferredLanguageId
        };
}
