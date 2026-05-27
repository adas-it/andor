using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Tests.Currencies;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class AccountNewAsyncTests
{
    private readonly Mock<IAccountValidator> _validatorMock;

    public AccountNewAsyncTests()
    {
        _validatorMock = new Mock<IAccountValidator>();
    }

    #region NewAsync - Success Cases

    [Fact]
    public async Task NewAsync_WithValidData_ShouldCreateAccountSuccessfully()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        Assert.Equal(accountId, account.Id);
        Assert.Equal(name, account.Name);
        Assert.Equal(description, account.Description);
        Assert.Equal(currency, account.Currency);
        Assert.False(account.IsDeleted);
    }

    [Fact]
    public async Task NewAsync_ShouldAddOwnerAsMember()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var ownerId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, ownerId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        _ = Assert.Single(account.Members);

        var ownerMember = account.Members.First();
        Assert.Equal(ownerId, ownerMember.UserId);
        Assert.Equal(PermissionType.Owner, ownerMember.PermissionType);
        Assert.Equal(1, ownerMember.Order);
    }

    [Fact]
    public async Task NewAsync_ShouldRaiseAccountCreatedDomainEvent()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        _ = Assert.Single(account.Events);

        var domainEvent = account.Events.First();
        _ = Assert.IsType<AccountCreatedDomainEvent>(domainEvent);

        var accountCreatedEvent = (AccountCreatedDomainEvent)domainEvent;
        Assert.Equal(userId, accountCreatedEvent.UserId);
        Assert.Equal(name.Value, accountCreatedEvent.Name);
    }

    [Fact]
    public async Task NewAsync_ShouldInitializeEmptyCollections()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        Assert.Empty(account.Categories);
        Assert.Empty(account.SubCategories);
        Assert.Empty(account.PaymentMethods);
        Assert.Empty(account.Invites);
        _ = Assert.Single(account.Members); // Only owner
    }

    [Fact]
    public async Task NewAsync_WithDifferentUserIds_ShouldCreateDifferentOwners()
    {
        // Arrange
        var accountId1 = AccountId.New();
        var accountId2 = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (_, account1) = await Account.NewAsync(
            accountId1, name, description, currency, userId1, _validatorMock.Object, CancellationToken.None);

        var (_, account2) = await Account.NewAsync(
            accountId2, name, description, currency, userId2, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(account1);
        Assert.NotNull(account2);
        Assert.NotEqual(account1.Members.First().UserId, account2.Members.First().UserId);
        Assert.Equal(userId1, account1.Members.First().UserId);
        Assert.Equal(userId2, account2.Members.First().UserId);
    }

    #endregion

    #region NewAsync - Validation Failure Cases

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnFailure("Validation error");

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(account);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnFailure("Validation error");

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(account);
    }

    [Fact]
    public async Task NewAsync_ShouldCallValidatorWithCorrectAccount()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        Account? capturedAccount = null;

        _ = _validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .Callback<Account, CancellationToken>((acc, ct) => capturedAccount = acc)
            .ReturnsAsync(new List<Notification>());

        // Act
        _ = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, cancellationToken);

        // Assert
        _validatorMock.Verify(v => v.ValidateCreationAsync(It.IsAny<Account>(), cancellationToken), Times.Once);
        Assert.NotNull(capturedAccount);
        Assert.Equal(accountId, capturedAccount!.Id);
        Assert.Equal(name, capturedAccount.Name);
    }

    #endregion

    #region NewAsync - Edge Cases

    [Fact]
    public async Task NewAsync_WithMinimumValidName_ShouldCreateSuccessfully()
    {
        // Arrange
        var accountId = AccountId.New();
        Name name = "ABC"; // Minimum 3 characters
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        Assert.Equal("ABC", account.Name.Value);
    }

    [Fact]
    public async Task NewAsync_WithMaximumValidName_ShouldCreateSuccessfully()
    {
        // Arrange
        var accountId = AccountId.New();
        var maxName = new string('A', Name.MaxLength);
        Name name = maxName;
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        Assert.Equal(maxName, account.Name.Value);
    }

    [Fact]
    public async Task NewAsync_WithEmptyCurrency_ShouldStillCreate()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = Currency.Empty;
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        Assert.Equal(Currency.Empty, account.Currency);
    }

    [Fact]
    public async Task NewAsync_WithDifferentAccountIds_ShouldCreateDifferentAccounts()
    {
        // Arrange
        var accountId1 = AccountId.New();
        var accountId2 = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (_, account1) = await Account.NewAsync(
            accountId1, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        var (_, account2) = await Account.NewAsync(
            accountId2, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(account1);
        Assert.NotNull(account2);
        Assert.NotEqual(account1.Id, account2.Id);
    }

    #endregion

    #region NewAsync - IsDeleted State

    [Fact]
    public async Task NewAsync_ShouldCreateAccountWithIsDeletedFalse()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        SetupValidatorToReturnSuccess();

        // Act
        var (result, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(account);
        Assert.False(account.IsDeleted);
    }

    #endregion

    #region NewAsync - CancellationToken

    [Fact]
    public async Task NewAsync_ShouldPassCancellationTokenToValidator()
    {
        // Arrange
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _ = _validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), cancellationToken))
            .ReturnsAsync(new List<Notification>());

        // Act
        _ = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, cancellationToken);

        // Assert
        _validatorMock.Verify(v => v.ValidateCreationAsync(It.IsAny<Account>(), cancellationToken), Times.Once);
    }

    #endregion

    #region Helper Methods

    private void SetupValidatorToReturnSuccess()
    {
        _ = _validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
    }

    private void SetupValidatorToReturnFailure(string errorMessage)
    {
        var notification = new Notification("Field", errorMessage, DomainErrorCode.Validation);

        _ = _validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { notification });
    }

    #endregion
}
