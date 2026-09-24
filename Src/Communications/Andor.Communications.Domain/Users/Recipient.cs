using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.Users;

/// <summary>
/// Local copy of a Users-module User, kept in sync by a Service Bus consumer reacting to
/// UserCreatedDomainEvent, so callers requesting a communication don't need to resend
/// Name/Email/PreferredLanguage on every request and so Marketing sends can be gated on consent
/// without a synchronous call back to Users.Service. <see cref="Id"/> equals the User's own id
/// (see <see cref="RecipientId.Load(Guid)"/>) rather than a locally minted one, since this is a
/// 1:1 projection, not an aggregate with its own identity.
/// </summary>
public class Recipient : Entity<RecipientId>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Guid PreferredLanguageId { get; private set; }
    public bool Active { get; private set; }
    public bool MarketingOptIn { get; private set; }
    public bool TermsAndConditionsAccepted { get; private set; }
    public bool PrivacyPolicyAccepted { get; private set; }

    private Recipient()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    private Recipient(
        RecipientId id,
        string name,
        string email,
        Guid preferredLanguageId,
        bool active,
        bool marketingOptIn,
        bool termsAndConditionsAccepted,
        bool privacyPolicyAccepted)
    {
        Id = id;
        Name = name;
        Email = email;
        PreferredLanguageId = preferredLanguageId;
        Active = active;
        MarketingOptIn = marketingOptIn;
        TermsAndConditionsAccepted = termsAndConditionsAccepted;
        PrivacyPolicyAccepted = privacyPolicyAccepted;
    }

    public static (DomainResult, Recipient?) New(
        RecipientId id,
        string name,
        string email,
        Guid preferredLanguageId,
        bool active,
        bool marketingOptIn,
        bool termsAndConditionsAccepted,
        bool privacyPolicyAccepted)
    {
        var entity = new Recipient(id, name, email, preferredLanguageId, active,
            marketingOptIn, termsAndConditionsAccepted, privacyPolicyAccepted);

        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    /// <summary>
    /// Re-applies the latest known state from Users.Service — used when the sync consumer
    /// receives a message for a Recipient it already has (e.g. a redelivered event).
    /// </summary>
    public DomainResult Update(
        string name,
        string email,
        Guid preferredLanguageId,
        bool active,
        bool marketingOptIn,
        bool termsAndConditionsAccepted,
        bool privacyPolicyAccepted)
    {
        Name = name;
        Email = email;
        PreferredLanguageId = preferredLanguageId;
        Active = active;
        MarketingOptIn = marketingOptIn;
        TermsAndConditionsAccepted = termsAndConditionsAccepted;
        PrivacyPolicyAccepted = privacyPolicyAccepted;

        return Validate();
    }

    protected override DomainResult Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(2, 50));

        return base.Validate();
    }
}
