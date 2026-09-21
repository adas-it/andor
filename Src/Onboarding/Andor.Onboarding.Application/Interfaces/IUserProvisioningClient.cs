using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Onboarding.Application.Interfaces;

/// <summary>
/// Synchronous call into Users.Service (POST /v1/users) that provisions the User — and, from
/// there, Identity credentials and a default Account — as part of confirming a signup. Kept as
/// a hard, synchronous dependency of Verify because those three are a "must happen" chain, unlike
/// the best-effort welcome-email notification that still flows through the existing event.
/// </summary>
public interface IUserProvisioningClient
{
    Task<DomainResult> ProvisionAsync(
        Guid userId,
        string name,
        string email,
        string passwordHash,
        bool marketingOptIn,
        bool termsAndConditionsAccepted,
        bool privacyPolicyAccepted,
        CancellationToken cancellationToken);
}
