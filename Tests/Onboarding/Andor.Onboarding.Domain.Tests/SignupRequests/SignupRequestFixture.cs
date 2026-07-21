using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.ValueObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Onboarding.Domain.Tests.SignupRequests;

/// <summary>
/// Fixture for creating SignupRequest instances for testing purposes.
/// </summary>
internal static class SignupRequestFixture
{
    public static async Task<(DomainResult result, SignupRequest? signupRequest)> CreateValidSignupRequestAsync(
        Mock<IOnboardingValidator>? validatorMock = null,
        SignupRequestId? id = null,
        Name? name = null,
        string? email = null,
        CancellationToken cancellationToken = default)
    {
        var validator = validatorMock ?? CreateDefaultValidator();

        var result = await SignupRequest.NewAsync(
            id ?? SignupRequestId.New(),
            name ?? GeneralFixture.GetValidName(),
            email ?? "someone@example.com",
            validator.Object,
            cancellationToken);

        return result;
    }

    public static Mock<IOnboardingValidator> CreateDefaultValidator()
    {
        var validatorMock = new Mock<IOnboardingValidator>();
        _ = validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<SignupRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
        return validatorMock;
    }

    public static Mock<IOnboardingValidator> CreateFailingValidator(List<Notification> notifications)
    {
        var validatorMock = new Mock<IOnboardingValidator>();
        _ = validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<SignupRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);
        return validatorMock;
    }
}
