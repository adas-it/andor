using Andor.Users.Domain.Tests.Users;
using Moq;
using System.Net.Mail;
using Users.Users;
using Users.Users.DomainEvents;
using Users.Users.Errors;
using Users.Users.ValueObjects;

namespace Andor.Users.Domain.Tests;

public class UsersUnitTests
{
    #region NewAsync - Success Cases

    [Fact]
    public async Task NewAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Act
        var (result, user) = await UserFixture.CreateValidUserAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(user);
    }

    [Fact]
    public async Task NewAsync_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var userId = UserId.New();
        var email = new MailAddress("jane.doe@example.com");
        var firstName = "Jane";
        var lastName = "Doe";
        var currencyId = Guid.NewGuid();
        var languageId = Guid.NewGuid();

        // Act
        var (result, user) = await UserFixture.CreateValidUserAsync(
            userId: userId,
            email: email,
            firstName: firstName,
            lastName: lastName,
            preferredCurrencyId: currencyId,
            preferredLanguageId: languageId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(currencyId, user.PreferredCurrencyId);
        Assert.Equal(languageId, user.PreferredLanguageId);
    }

    [Fact]
    public async Task NewAsync_ShouldRaiseUserCreatedDomainEvent()
    {
        // Act
        var (result, user) = await UserFixture.CreateValidUserAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(user);
        Assert.Contains(user.Events, e => e is UserCreatedDomainEvent);
    }

    [Fact]
    public async Task NewAsync_DomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var email = new MailAddress("events@example.com");
        var firstName = "Event";
        var lastName = "User";
        var currencyId = Guid.NewGuid();
        var languageId = Guid.NewGuid();

        // Act
        var (_, user) = await UserFixture.CreateValidUserAsync(
            email: email,
            firstName: firstName,
            lastName: lastName,
            preferredCurrencyId: currencyId,
            preferredLanguageId: languageId);

        // Assert
        Assert.NotNull(user);
        var domainEvent = Assert.Single(user.Events.OfType<UserCreatedDomainEvent>());
        Assert.Equal(email.Address, domainEvent.Email);
        Assert.Equal(firstName, domainEvent.FirstName);
        Assert.Equal(lastName, domainEvent.LastName);
        Assert.Equal(currencyId, domainEvent.PreferredCurrencyId);
        Assert.Equal(languageId, domainEvent.PreferredLanguageId);
    }

    [Fact]
    public async Task NewAsync_TwoCalls_ShouldGenerateDifferentIds()
    {
        // Act
        var (_, user1) = await UserFixture.CreateValidUserAsync();
        var (_, user2) = await UserFixture.CreateValidUserAsync();

        // Assert
        Assert.NotNull(user1);
        Assert.NotNull(user2);
        Assert.NotEqual(user1.Id, user2.Id);
    }

    #endregion

    #region NewAsync - Email Uniqueness Validation

    [Fact]
    public async Task NewAsync_WithDuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var emailInUseValidator = UserFixture.CreateEmailInUseValidator();

        // Act
        var (result, user) = await UserFixture.CreateValidUserAsync(validator: emailInUseValidator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(user);
        Assert.Contains(result.Errors, e =>
            e.FieldName == nameof(User.Email) &&
            e.Error.Equals(UserErrorCode.EmailAlreadyInUse));
    }

    [Fact]
    public async Task NewAsync_WithDuplicateEmail_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var emailInUseValidator = UserFixture.CreateEmailInUseValidator();

        // Act
        var (_, user) = await UserFixture.CreateValidUserAsync(validator: emailInUseValidator);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task NewAsync_ValidatorIsCalledOnce()
    {
        // Arrange
        var validator = UserFixture.CreateDefaultValidator();

        // Act
        await UserFixture.CreateValidUserAsync(validator: validator);

        // Assert
        validator.Verify(
            v => v.ValidateCreationAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region NewAsync - CancellationToken

    [Fact]
    public async Task NewAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            UserFixture.CreateValidUserAsync(cancellationToken: cts.Token));
    }

    #endregion
}

