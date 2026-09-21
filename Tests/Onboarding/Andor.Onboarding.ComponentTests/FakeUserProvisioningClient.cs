using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Domain.Errors;

namespace Andor.Onboarding.ComponentTests;

/// <summary>
/// Replaces the real HTTP call to Users.Service in component tests: there's no such service
/// running in this test host, and unlike <see cref="Andor.ComponentTests.Common.NullMessageSender"/>
/// this one's a hard, synchronous dependency of Verify — a real HTTP failure would fail the test
/// outright, not just get silently dropped like an unsent notification.
/// </summary>
public sealed class FakeUserProvisioningClient : IUserProvisioningClient
{
    public bool ShouldFail { get; set; }

    public Task<DomainResult> ProvisionAsync(
        Guid userId,
        string name,
        string email,
        string passwordHash,
        bool marketingOptIn,
        bool termsAndConditionsAccepted,
        bool privacyPolicyAccepted,
        CancellationToken cancellationToken)
        => Task.FromResult(ShouldFail
            ? DomainResult.Failure(errors: new[]
            {
                new Notification(nameof(FakeUserProvisioningClient), "Simulated provisioning failure.",
                    SignupErrorCodes.ProvisioningFailed),
            })
            : DomainResult.Success());
}
