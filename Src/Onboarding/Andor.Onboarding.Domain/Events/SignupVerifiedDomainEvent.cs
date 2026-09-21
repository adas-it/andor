using Andor.Foundation.Domain.Events;

namespace Andor.Onboarding.Domain.Events;

public sealed record SignupVerifiedDomainEvent : DomainEvent
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required string PreferredLanguage { get; init; }
    public required bool MarketingOptIn { get; init; }
    public required bool TermsAndConditionsAccepted { get; init; }
    public required bool PrivacyPolicyAccepted { get; init; }

    public static SignupVerifiedDomainEvent FromSignupRequest(SignupRequest request, Guid userId, string passwordHash,
        bool marketingOptIn, bool termsAndConditionsAccepted, bool privacyPolicyAccepted) => new()
    {
        EventName = nameof(SignupVerifiedDomainEvent),
        Id = request.Id.Value,
        UserId = userId,
        Name = request.Name,
        Email = request.Email,
        PasswordHash = passwordHash,
        PreferredLanguage = request.PreferredLanguage,
        MarketingOptIn = marketingOptIn,
        TermsAndConditionsAccepted = termsAndConditionsAccepted,
        PrivacyPolicyAccepted = privacyPolicyAccepted,
    };
}
