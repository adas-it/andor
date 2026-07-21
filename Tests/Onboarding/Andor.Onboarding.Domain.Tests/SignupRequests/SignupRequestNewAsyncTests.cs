using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.Events;
using Andor.Onboarding.Domain.ValueObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Onboarding.Domain.Tests.SignupRequests;

public class SignupRequestNewAsyncTests
{
    private readonly Mock<IOnboardingValidator> _validatorMock;

    public SignupRequestNewAsyncTests()
    {
        _validatorMock = SignupRequestFixture.CreateDefaultValidator();
    }

    #region NewAsync - Success Cases

    [Fact]
    public async Task NewAsync_WithValidData_ShouldCreateSignupRequestSuccessfully()
    {
        // Arrange
        var id = SignupRequestId.New();
        var name = GeneralFixture.GetValidName();
        var email = "someone@example.com";

        // Act
        var (result, signupRequest) = await SignupRequest.NewAsync(
            id, name, email, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(signupRequest);
        Assert.Equal(id, signupRequest.Id);
        Assert.Equal(name.Value, signupRequest.Name.Value);
        Assert.Equal(email, signupRequest.Email);
        Assert.False(signupRequest.IsVerified);
    }

    [Fact]
    public async Task NewAsync_ShouldGenerateTenDigitVerificationCode()
    {
        // Act
        var (result, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(signupRequest);
        Assert.Equal(10, signupRequest.VerificationCode.Length);
        Assert.True(long.TryParse(signupRequest.VerificationCode, out _));
    }

    [Fact]
    public async Task NewAsync_ShouldSetExpiresAtFifteenMinutesFromNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddMinutes(15);

        // Act
        var (result, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);
        var after = DateTime.UtcNow.AddMinutes(15);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(signupRequest);
        Assert.InRange(signupRequest.ExpiresAt, before, after);
    }

    [Fact]
    public async Task NewAsync_ShouldRaiseSignupCodeGeneratedDomainEvent()
    {
        // Act
        var (result, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(signupRequest);
        var domainEvent = Assert.Single(signupRequest.Events);
        var codeGenerated = Assert.IsType<SignupCodeGenerated>(domainEvent);
        Assert.Equal(signupRequest.Name.Value, codeGenerated.Name);
        Assert.Equal(signupRequest.Email, codeGenerated.Email);
        Assert.Equal(signupRequest.VerificationCode, codeGenerated.Code);
    }

    [Fact]
    public async Task NewAsync_ShouldRouteSignupCodeGeneratedToVerificationQueue()
    {
        // Act
        var (result, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(_validatorMock);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(signupRequest);
        var codeGenerated = Assert.IsType<SignupCodeGenerated>(signupRequest.Events.Single());
        Assert.Equal("signup-verification-codes", codeGenerated.QueueName);
    }

    #endregion

    #region NewAsync - Validation Failure Cases

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldReturnFailureAndNullEntity()
    {
        // Arrange
        var notification = new Notification("Email", "invalid", CommonErrorCodes.Validation);
        var failingValidator = SignupRequestFixture.CreateFailingValidator([notification]);

        // Act
        var (result, signupRequest) = await SignupRequestFixture.CreateValidSignupRequestAsync(failingValidator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(signupRequest);
    }

    #endregion
}
