using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Tests.Currencies;
using Andor.Accounts.Domain.Users;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class AccountCreateCustomCategoryTests
{
    private readonly Mock<IAccountValidator> _validatorMock;

    public AccountCreateCustomCategoryTests()
    {
        _validatorMock = new Mock<IAccountValidator>();
        SetupValidatorToReturnSuccess();
    }

    #region Success Cases

    [Fact]
    public async Task CreateCustomCategory_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var account = await CreateValidAccount();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.CreateCustomCategory(name, description, MovementType.MoneySpending, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
        Assert.False(account.Categories.First().Category.IsTemplate);
        Assert.Equal(account.Id, account.Categories.First().Category.Owner);
    }

    [Fact]
    public async Task CreateCustomCategory_ShouldSetCorrectOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.CreateCustomCategory(name, description, MovementType.MoneySpending, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, account.Categories.First().Order);
    }

    [Fact]
    public async Task CreateCustomCategory_MultipleTimes_ShouldIncrementOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var ownerUserId = account.Members.First().UserId;

        // Act
        _ = account.CreateCustomCategory("Category 1", "Description 1", MovementType.MoneySpending, ownerUserId);
        _ = account.CreateCustomCategory("Category 2", "Description 2", MovementType.MoneySpending, ownerUserId);
        _ = account.CreateCustomCategory("Category 3", "Description 3", MovementType.MoneySpending, ownerUserId);

        // Assert
        Assert.Equal(3, account.Categories.Count);
        Assert.Equal(1, account.Categories.ElementAt(0).Order);
        Assert.Equal(2, account.Categories.ElementAt(1).Order);
        Assert.Equal(3, account.Categories.ElementAt(2).Order);
    }

    [Fact]
    public async Task CreateCustomCategory_ShouldRaiseDomainEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.CreateCustomCategory(name, description, MovementType.MoneySpending, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(account.Events, e => e is AccountCategoryAddedDomainEvent);

        var domainEvent = account.Events.OfType<AccountCategoryAddedDomainEvent>().First();
        Assert.Equal(ownerUserId, domainEvent.UserId);
    }

    [Fact]
    public async Task CreateCustomCategory_WithEditorPermission_ShouldSucceed()
    {
        // Arrange
        var (account, editorUserId) = await CreateAccountWithAdditionalMember(PermissionType.Editor);
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var result = account.CreateCustomCategory(name, description, MovementType.MoneySpending, editorUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
    }

    #endregion

    #region Permission Tests

    [Fact]
    public async Task CreateCustomCategory_WithViewerPermission_ShouldFail()
    {
        // Arrange
        var (account, viewerUserId) = await CreateAccountWithAdditionalMember(PermissionType.Viewer);
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var result = account.CreateCustomCategory(name, description, MovementType.MoneySpending, viewerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.InsufficientPermissions));
    }

    [Fact]
    public async Task CreateCustomCategory_WithUserNotMember_ShouldFail()
    {
        // Arrange
        var account = await CreateValidAccount();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var nonMemberUserId = Guid.NewGuid();

        // Act
        var result = account.CreateCustomCategory(name, description, MovementType.MoneySpending, nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.UserNotMember));
    }

    #endregion

    #region Helper Methods

    private async Task<Account> CreateValidAccount()
    {
        var accountId = AccountId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var currency = CurrencyFixture.GetUsdCurrency();
        var userId = Guid.NewGuid();

        var (_, account) = await Account.NewAsync(
            accountId, name, description, currency, userId, _validatorMock.Object, CancellationToken.None);

        return account!;
    }

    private async Task<(Account account, Guid userId)> CreateAccountWithAdditionalMember(PermissionType permissionType)
    {
        var account = await CreateValidAccount();

        var ownerUserId = account.Members.First().UserId;

        var additionalUserId = Guid.NewGuid();
        var additionalUser = new User() { Id = additionalUserId };
        _ = account.LinkMember(additionalUser, permissionType, ownerUserId);

        return (account, additionalUserId);
    }

    private void SetupValidatorToReturnSuccess()
    {
        _ = _validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
    }

    #endregion
}
