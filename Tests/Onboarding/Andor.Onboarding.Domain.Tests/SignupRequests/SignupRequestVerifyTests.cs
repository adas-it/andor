using Andor.Onboarding.Domain.Errors;
using Andor.Onboarding.Domain.Events;
using Andor.Shared.Lookups;
using Moq;

namespace Andor.Onboarding.Domain.Tests.SignupRequests;

public class SignupRequestVerifyTests
{
    private readonly Mock<IOnboardingValidator> _validatorMock;

    public SignupRequestVerifyTests()
    {
        _validatorMock = SignupRequestFixture.CreateDefaultValidator();
    }

    [Fact]
    public async Task Verify_WithCorrectCode_ShouldMarkAsVerifiedAndReturnSuccess()
    {
        // Arrange
        var (_, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);
        var code = signupRequest!.VerificationCode;

        // Act
        var result = signupRequest.Verify(code, "hashed-password", Guid.NewGuid(), "en", null, true, true, true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(signupRequest.IsVerified);
    }

    [Fact]
    public async Task Verify_WithCorrectCode_ShouldRaiseSignupVerifiedDomainEvent()
    {
        // Arrange
        var (_, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);
        var code = signupRequest!.VerificationCode;

        // Act
        var result = signupRequest.Verify(code, "hashed-password", Guid.NewGuid(), "br", null, true, true, false);

        // Assert
        Assert.True(result.IsSuccess);
        var domainEvent = Assert.Single(signupRequest.Events.OfType<SignupVerifiedDomainEvent>());
        Assert.NotEqual(Guid.Empty, domainEvent.UserId);
        Assert.Equal(signupRequest.Name.Value, domainEvent.Name);
        Assert.Equal(signupRequest.Email, domainEvent.Email);
        Assert.Equal("hashed-password", domainEvent.PasswordHash);
        Assert.Equal(Language.GetByISO("br").Id, domainEvent.PreferredLanguageId);
        Assert.True(domainEvent.MarketingOptIn);
        Assert.True(domainEvent.TermsAndConditionsAccepted);
        Assert.False(domainEvent.PrivacyPolicyAccepted);
    }

    [Fact]
    public async Task Verify_WithIncorrectCode_ShouldReturnFailure()
    {
        // Arrange
        var (_, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);

        // Act
        var result = signupRequest!.Verify("0000000000", "hashed-password", Guid.NewGuid(), "en", null, true, true, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(signupRequest.IsVerified);
        Assert.Contains(result.Errors, e => e.Error.Equals(SignupErrorCodes.InvalidCode));
    }

    [Fact]
    public async Task Verify_WithIncorrectCode_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var (_, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);

        // Act
        var result = signupRequest!.Verify("0000000000", "hashed-password", Guid.NewGuid(), "en", null, true, true, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Empty(signupRequest.Events.OfType<SignupVerifiedDomainEvent>());
    }

    [Fact]
    public async Task Verify_WhenAlreadyVerified_ShouldReturnFailure()
    {
        // Arrange
        var (_, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);
        var code = signupRequest!.VerificationCode;
        _ = signupRequest.Verify(code, "hashed-password", Guid.NewGuid(), "en", null, true, true, true);

        // Act - verify again with the same (still correct) code
        var result = signupRequest.Verify(code, "hashed-password", Guid.NewGuid(), "en", null, true, true, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Error.Equals(SignupErrorCodes.AlreadyVerified));
        Assert.Single(signupRequest.Events.OfType<SignupVerifiedDomainEvent>());
    }

}
