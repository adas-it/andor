namespace Andor.Users.Contracts.Requests;

public record CreateUserInput(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string PasswordHash,
    bool MarketingOptIn,
    bool TermsAndConditionsAccepted,
    bool PrivacyPolicyAccepted);
