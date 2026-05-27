using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Tests.Categories;
using Andor.Accounts.Domain.Tests.Currencies;
using Andor.Accounts.Domain.Users;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class AccountAddTemplateCategoryTests
{
    private readonly Mock<IAccountValidator> _validatorMock;

    public AccountAddTemplateCategoryTests()
    {
        _validatorMock = new Mock<IAccountValidator>();
        SetupValidatorToReturnSuccess();
    }

    #region Success Cases

    [Fact]
    public async Task AddTemplateCategory_WithValidTemplateCategory_ShouldAddSuccessfully()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
        Assert.Equal(category.Id, account.Categories.First().CategoryId);
    }

    [Fact]
    public async Task AddTemplateCategory_ShouldSetCorrectOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, account.Categories.First().Order);
    }

    [Fact]
    public async Task AddTemplateCategory_MultipleTimes_ShouldIncrementOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category1 = CategoryFixture.GetTemplateCategory();
        var category2 = CategoryFixture.GetTemplateCategory();
        var category3 = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        _ = account.AddTemplateCategory(category1, ownerUserId);
        _ = account.AddTemplateCategory(category2, ownerUserId);
        _ = account.AddTemplateCategory(category3, ownerUserId);

        // Assert
        Assert.Equal(3, account.Categories.Count);
        Assert.Equal(1, account.Categories.ElementAt(0).Order);
        Assert.Equal(2, account.Categories.ElementAt(1).Order);
        Assert.Equal(3, account.Categories.ElementAt(2).Order);
    }

    [Fact]
    public async Task AddTemplateCategory_ShouldRaiseDomainEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(account.Events, e => e is AccountCategoryAddedDomainEvent);

        var domainEvent = account.Events.OfType<AccountCategoryAddedDomainEvent>().First();
        Assert.Equal(ownerUserId, domainEvent.UserId);
    }

    [Fact]
    public async Task AddTemplateCategory_WithDifferentCategories_ShouldAddAll()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category1 = CategoryFixture.GetTemplateCategory(name: "Category 1");
        var category2 = CategoryFixture.GetTemplateCategory(name: "Category 2");
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result1 = account.AddTemplateCategory(category1, ownerUserId);
        var result2 = account.AddTemplateCategory(category2, ownerUserId);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal(2, account.Categories.Count);
    }

    #endregion

    #region Validation - Null Category

    [Fact]
    public async Task AddTemplateCategory_WithNullCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        Category? category = null;
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category!, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.CategoryCannotBeNull));
        Assert.Empty(account.Categories);
    }

    [Fact]
    public async Task AddTemplateCategory_WithNullCategory_ShouldNotRaiseEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        Category? category = null;
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category!, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.DoesNotContain(account.Events, e => e is AccountCategoryAddedDomainEvent);
    }

    #endregion

    #region Validation - IsTemplate

    [Fact]
    public async Task AddTemplateCategory_WithCustomCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var owner = AccountId.New();
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error == AccountErrorCode.CategoryMustBeTemplate);
        Assert.Empty(account.Categories);
    }

    [Fact]
    public async Task AddTemplateCategory_WithCustomCategory_ShouldNotRaiseEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var owner = AccountId.New();
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.DoesNotContain(account.Events, e => e is AccountCategoryAddedDomainEvent);
    }

    #endregion

    #region Validation - Duplicate Category

    [Fact]
    public async Task AddTemplateCategory_WithDuplicateCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // First add
        _ = account.AddTemplateCategory(category, ownerUserId);

        // Act - Try to add same category again
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.CategoryAlreadyAdded));
        _ = Assert.Single(account.Categories); // Should still have only 1
    }

    [Fact]
    public async Task AddTemplateCategory_AddingSameCategoryTwice_ShouldOnlyAddOnce()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result1 = account.AddTemplateCategory(category, ownerUserId);
        var result2 = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsFailure);
        _ = Assert.Single(account.Categories);
    }

    #endregion

    #region Validation - Deleted Category

    [Fact]
    public async Task AddTemplateCategory_WithDeletedCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        _ = category.SoftDelete();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.CannotAddDeletedCategory));
        Assert.Empty(account.Categories);
    }

    [Fact]
    public async Task AddTemplateCategory_WithDeletedCategory_ShouldNotRaiseEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        _ = category.SoftDelete();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.DoesNotContain(account.Events, e => e is AccountCategoryAddedDomainEvent);
    }

    #endregion

    #region Order Management Tests

    [Fact]
    public async Task AddTemplateCategory_FirstCategory_ShouldHaveOrder1()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        _ = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.Equal(1, account.Categories.First().Order);
    }

    [Fact]
    public async Task AddTemplateCategory_AfterMultipleAdds_ShouldUseMaxOrderPlusOne()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category1 = CategoryFixture.GetTemplateCategory();
        var category2 = CategoryFixture.GetTemplateCategory();
        var category3 = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        _ = account.AddTemplateCategory(category1, ownerUserId);
        _ = account.AddTemplateCategory(category2, ownerUserId);
        _ = account.AddTemplateCategory(category3, ownerUserId);

        // Assert
        var orders = account.Categories.Select(c => c.Order).ToList();
        Assert.Equal(new[] { 1, 2, 3 }, orders);
    }

    [Fact]
    public async Task AddTemplateCategory_ShouldMaintainSequentialOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var ownerUserId = account.Members.First().UserId;

        // Act - Add 5 categories
        for (int i = 0; i < 5; i++)
        {
            var category = CategoryFixture.GetTemplateCategory();
            _ = account.AddTemplateCategory(category, ownerUserId);
        }

        // Assert
        var orders = account.Categories.Select(c => c.Order).OrderBy(o => o).ToList();
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, orders);
    }

    #endregion

    #region Domain Events Tests

    [Fact]
    public async Task AddTemplateCategory_MultipleTimes_ShouldRaiseMultipleEvents()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category1 = CategoryFixture.GetTemplateCategory();
        var category2 = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        _ = account.AddTemplateCategory(category1, ownerUserId);
        _ = account.AddTemplateCategory(category2, ownerUserId);

        // Assert
        var events = account.Events.OfType<AccountCategoryAddedDomainEvent>().ToList();
        Assert.Equal(2, events.Count);
    }

    [Fact]
    public async Task AddTemplateCategory_WithDifferentUserIds_ShouldTrackCorrectUserId()
    {
        // Arrange
        var (account, editorUserId) = await CreateAccountWithAdditionalMember(PermissionType.Editor);
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First(m => m.PermissionType == PermissionType.Owner).UserId;

        // Act
        _ = account.AddTemplateCategory(category, ownerUserId);

        var category2 = CategoryFixture.GetTemplateCategory();
        _ = account.AddTemplateCategory(category2, editorUserId);

        // Assert
        var events = account.Events.OfType<AccountCategoryAddedDomainEvent>().ToList();
        Assert.Equal(ownerUserId, events[0].UserId);
        Assert.Equal(editorUserId, events[1].UserId);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task AddTemplateCategory_WithCategoryNameAtMinLength_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccount();
        Name name = "ABC"; // Minimum 3 characters
        var category = CategoryFixture.GetTemplateCategory(name: name);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
    }

    [Fact]
    public async Task AddTemplateCategory_WithCategoryNameAtMaxLength_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccount();
        var maxName = new string('A', Name.MaxLength);
        Name name = maxName;
        var category = CategoryFixture.GetTemplateCategory(name: name);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
    }

    #endregion

    #region Permission Validation

    [Fact]
    public async Task AddTemplateCategory_WithUserNotMember_ShouldFail()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var nonMemberUserId = Guid.NewGuid();

        // Act
        var result = account.AddTemplateCategory(category, nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, n =>
            n.FieldName == "userId" &&
            n.Error == AccountErrorCode.UserNotMember);
        Assert.Empty(account.Categories);
    }

    [Fact]
    public async Task AddTemplateCategory_WithViewerPermission_ShouldFail()
    {
        // Arrange
        var (account, viewerUserId) = await CreateAccountWithAdditionalMember(PermissionType.Viewer);
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var result = account.AddTemplateCategory(category, viewerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, n =>
            n.FieldName == "userId" &&
            n.Error == AccountErrorCode.InsufficientPermissions);
        Assert.Empty(account.Categories);
    }

    [Fact]
    public async Task AddTemplateCategory_WithEditorPermission_ShouldSucceed()
    {
        // Arrange
        var (account, editorUserId) = await CreateAccountWithAdditionalMember(PermissionType.Editor);
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var result = account.AddTemplateCategory(category, editorUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
        Assert.Equal(category.Id, account.Categories.First().CategoryId);
    }

    [Fact]
    public async Task AddTemplateCategory_WithOwnerPermission_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();

        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
        Assert.Equal(category.Id, account.Categories.First().CategoryId);
    }

    [Fact]
    public async Task AddTemplateCategory_WithInsufficientPermissions_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var (account, viewerUserId) = await CreateAccountWithAdditionalMember(PermissionType.Viewer);
        var category = CategoryFixture.GetTemplateCategory();
        var initialEventCount = account.Events.Count;

        // Act
        var result = account.AddTemplateCategory(category, viewerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.DoesNotContain(account.Events.Skip(initialEventCount), e => e is AccountCategoryAddedDomainEvent);
    }

    [Fact]
    public async Task AddTemplateCategory_WithNonMember_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var initialEventCount = account.Events.Count;
        var nonMemberUserId = Guid.NewGuid();

        // Act
        var result = account.AddTemplateCategory(category, nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.DoesNotContain(account.Events.Skip(initialEventCount), e => e is AccountCategoryAddedDomainEvent);
    }

    [Fact]
    public async Task AddTemplateCategory_WithMultipleValidationErrors_ShouldIncludePermissionError()
    {
        // Arrange
        var account = await CreateValidAccount();
        var nonMemberUserId = Guid.NewGuid();

        // Act - 
        var result = account.AddTemplateCategory(null!, nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, n =>
            n.FieldName == "userId" &&
            n.Error == AccountErrorCode.UserNotMember);
        Assert.Contains(result.Errors, n =>
            n.FieldName == nameof(Category) &&
            n.Error == AccountErrorCode.CategoryCannotBeNull);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task AddTemplateCategory_ToEmptyAccount_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccount();
        Assert.Empty(account.Categories);

        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateCategory(category, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.Categories);
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

