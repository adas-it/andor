using Andor.Foundation.Domain.Events;

namespace Andor.Users.Domain.Users.DomainEvents;

public record UserCreatedDomainEvent : DomainEvent
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid PreferredCurrencyId { get; set; }
    public Guid PreferredLanguageId { get; set; }
    public bool MarketingOptIn { get; set; }
    public bool TermsAndConditionsAccepted { get; set; }
    public bool PrivacyPolicyAccepted { get; set; }

    public static UserCreatedDomainEvent FromAggregateRoot(User entity)
        => new UserCreatedDomainEvent() with
        {
            EventName = nameof(UserCreatedDomainEvent),
            Id = entity.Id,
            UserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Value,
            PreferredCurrencyId = (Guid)entity.PreferredCurrencyId,
            PreferredLanguageId = (Guid)entity.PreferredLanguageId,
            MarketingOptIn = entity.MarketingOptIn,
            TermsAndConditionsAccepted = entity.TermsAndConditionsAccepted,
            PrivacyPolicyAccepted = entity.PrivacyPolicyAccepted
        };
}
