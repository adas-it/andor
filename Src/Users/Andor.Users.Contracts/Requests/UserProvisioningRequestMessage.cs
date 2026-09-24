namespace Andor.Users.Contracts.Requests;

public sealed record UserProvisioningRequestMessage(
    Guid UserId,
    string Name,
    string Email,
    string PasswordHash,
    Guid PreferredLanguageId,
    Guid PreferredCurrencyId,
    bool MarketingOptIn,
    bool TermsAndConditionsAccepted,
    bool PrivacyPolicyAccepted);
