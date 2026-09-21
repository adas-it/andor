using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.Errors;
using Andor.Onboarding.Domain.Events;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Domain;

/// <summary>
/// Represents a pending sign-up started from the public landing page: a name/email pair
/// waiting for the 6-digit code (sent by e-mail) to be confirmed alongside a password.
/// </summary>
public class SignupRequest : AggregateRoot<SignupRequestId>
{
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public VerificationCode VerificationCode { get; private set; }
    public string PreferredLanguage { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Default parameter less constructor for ORM usage.
    /// </summary>
    protected SignupRequest()
    {
        Name = Name.Empty;
        Email = Email.Empty;
        VerificationCode = VerificationCode.Empty;
        PreferredLanguage = string.Empty;
    }

    private SignupRequest(
        SignupRequestId id,
        Name name,
        Email email,
        VerificationCode verificationCode,
        string preferredLanguage,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        VerificationCode = verificationCode;
        PreferredLanguage = preferredLanguage;
        CreatedAt = createdAt;
        IsVerified = false;
    }

    /// <summary>
    /// Starts a new signup request: generates the 6-digit verification code and, on
    /// success, raises <see cref="SignupCodeGenerated"/> so the code gets e-mailed to the
    /// user via the Communications module.
    /// </summary>
    public static async Task<(DomainResult, SignupRequest?)> NewAsync(
        SignupRequestId id,
        Name name,
        Email email,
        string preferredLanguage,
        IOnboardingValidator validator,
        CancellationToken cancellationToken)
    {
        var code = VerificationCode.New();

        var entity = new SignupRequest(
            id,
            name,
            email,
            code,
            preferredLanguage,
            DateTime.UtcNow);

        var result = await entity.ValidateAsync(validator, cancellationToken);

        if (result.IsSuccess)
        {
            entity.RaiseDomainEvent(SignupCodeGenerated.FromSignupRequest(entity));
        }

        return (result, result.IsSuccess ? entity : null);
    }

    /// <summary>
    /// Restarts a still-pending sign-up request: regenerates the verification code (and
    /// refreshes the name, in case it changed) and, on success, raises
    /// <see cref="SignupCodeGenerated"/> again so a fresh code gets e-mailed. This is the
    /// only way to invalidate a previously issued code — codes don't expire on their own.
    /// </summary>
    public async Task<DomainResult> RestartAsync(Name name, string preferredLanguage, IOnboardingValidator validator, CancellationToken cancellationToken)
    {
        if (IsVerified)
        {
            AddNotification(nameof(IsVerified), "This sign-up request was already verified.", SignupErrorCodes.AlreadyVerified);
            return Validate();
        }

        Name = name;
        PreferredLanguage = preferredLanguage;
        VerificationCode = VerificationCode.New();

        var result = await ValidateAsync(validator, cancellationToken);

        if (result.IsSuccess)
        {
            RaiseDomainEvent(SignupCodeGenerated.FromSignupRequest(this));
        }

        return result;
    }

    /// <summary>
    /// Read-only precondition check for <see cref="Verify"/> — lets a caller that needs to do
    /// I/O before committing (e.g. provisioning the User over HTTP) bail out early on a stale or
    /// wrong code without paying for that I/O first.
    /// </summary>
    public DomainResult CanVerify(VerificationCode code)
    {
        if (IsVerified)
        {
            AddNotification(nameof(IsVerified), "This sign-up request was already verified.", SignupErrorCodes.AlreadyVerified);
            return Validate();
        }

        if (VerificationCode != code)
        {
            AddNotification(nameof(VerificationCode), "The verification code is invalid.", SignupErrorCodes.InvalidCode);
            return Validate();
        }

        return DomainResult.Success();
    }

    /// <summary>
    /// Confirms the code and, on success, raises <see cref="SignupVerifiedDomainEvent"/> (carrying
    /// the given, already-provisioned user id and the already-hashed password) so Communications
    /// can react to it too. <paramref name="userId"/> is supplied rather than minted here because
    /// by the time this is called, the User it refers to must already exist — see
    /// <see cref="CanVerify"/> for the check a caller should run before doing that provisioning.
    /// </summary>
    public DomainResult Verify(VerificationCode code, string passwordHash, Guid userId, string preferredLanguage,
        bool marketingOptIn, bool termsAndConditionsAccepted, bool privacyPolicyAccepted)
    {
        var precondition = CanVerify(code);

        if (precondition.IsFailure)
        {
            return precondition;
        }

        if (!string.IsNullOrWhiteSpace(preferredLanguage))
        {
            PreferredLanguage = preferredLanguage;
        }

        IsVerified = true;
        RaiseDomainEvent(SignupVerifiedDomainEvent.FromSignupRequest(this, userId, passwordHash,
            marketingOptIn, termsAndConditionsAccepted, privacyPolicyAccepted));

        return DomainResult.Success();
    }
}
