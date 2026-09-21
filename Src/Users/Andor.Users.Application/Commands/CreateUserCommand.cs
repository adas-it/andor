using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Application.Commands;

public record CreateUserCommand(
    UserId Id,
    Email Email,
    string FirstName,
    string LastName,
    Guid PreferredCurrencyId,
    Guid PreferredLanguageId,
    bool MarketingOptIn,
    bool TermsAndConditionsAccepted,
    bool PrivacyPolicyAccepted,
    // Not persisted on User (that's Identity's own concern) — carried only so CreateUserAsync
    // can hand it off on the "request-identity-user" queue after the User is provisioned.
    string PasswordHash,
    CancellationToken CancellationToken);
