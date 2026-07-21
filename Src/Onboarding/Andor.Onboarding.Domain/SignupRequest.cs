using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.Errors;
using Andor.Onboarding.Domain.Events;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Domain;

/// <summary>
/// Represents a pending signup started from the public landing page: a name/email pair
/// waiting for the 10-digit code (sent by e-mail) to be confirmed alongside a password.
/// </summary>
public class SignupRequest : AggregateRoot<SignupRequestId>
{
    public Name Name { get; private set; }
    public string Email { get; private set; }
    public string VerificationCode { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Default parameterless constructor for ORM usage.
    /// </summary>
    private SignupRequest()
    {
        Name = Name.Empty;
        Email = string.Empty;
        VerificationCode = string.Empty;
    }

    private SignupRequest(
        SignupRequestId id,
        Name name,
        string email,
        string verificationCode,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        VerificationCode = verificationCode;
        CreatedAt = createdAt;
        IsVerified = false;
    }

    /// <summary>
    /// Starts a new signup request: generates the 10-digit verification code and, on
    /// success, raises <see cref="SignupCodeGenerated"/> so the code gets e-mailed to the
    /// user via the Communications module.
    /// </summary>
    public static async Task<(DomainResult, SignupRequest?)> NewAsync(
        SignupRequestId id,
        Name name,
        string email,
        IOnboardingValidator validator,
        CancellationToken cancellationToken)
    {
        var code = Random.Shared.NextInt64(0, 10_000_000_000).ToString("D10");

        var entity = new SignupRequest(
            id,
            name,
            email,
            code,
            DateTime.UtcNow);

        var result = await entity.ValidateAsync(validator, cancellationToken);

        if (result.IsSuccess)
        {
            entity.RaiseDomainEvent(SignupCodeGenerated.FromSignupRequest(entity));
        }

        return (result, result.IsSuccess ? entity : null);
    }

    /// <summary>
    /// Restarts a still-pending signup request: regenerates the verification code (and
    /// refreshes the name, in case it changed) and, on success, raises
    /// <see cref="SignupCodeGenerated"/> again so a fresh code gets e-mailed. This is the
    /// only way to invalidate a previously issued code — codes don't expire on their own.
    /// </summary>
    public async Task<DomainResult> RestartAsync(Name name, IOnboardingValidator validator, CancellationToken cancellationToken)
    {
        if (IsVerified)
        {
            AddNotification(nameof(IsVerified), "This signup request was already verified.", SignupErrorCodes.AlreadyVerified);
            return Validate();
        }

        Name = name;
        VerificationCode = Random.Shared.NextInt64(0, 10_000_000_000).ToString("D10");

        var result = await ValidateAsync(validator, cancellationToken);

        if (result.IsSuccess)
        {
            RaiseDomainEvent(SignupCodeGenerated.FromSignupRequest(this));
        }

        return result;
    }

    /// <summary>
    /// Confirms the code and, on success, raises <see cref="SignupVerifiedDomainEvent"/>
    /// (carrying a freshly minted user id and the already-hashed password) so Identity and
    /// Accounts can each create their own records from the same event.
    /// </summary>
    public DomainResult Verify(string code, string passwordHash)
    {
        if (IsVerified)
        {
            AddNotification(nameof(IsVerified), "This signup request was already verified.", SignupErrorCodes.AlreadyVerified);
            return Validate();
        }

        if (VerificationCode != code)
        {
            AddNotification(nameof(VerificationCode), "The verification code is invalid.", SignupErrorCodes.InvalidCode);
            return Validate();
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            IsVerified = true;
            RaiseDomainEvent(SignupVerifiedDomainEvent.FromSignupRequest(this, Guid.NewGuid(), passwordHash));
        }

        return result;
    }
}
