using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.Tests.Categories;
using Andor.Accounts.Domain.Tests.Currencies;
using Andor.Accounts.Domain.Tests.SubCategories;
using Andor.Accounts.Domain.Users;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class AccountAddTemplateSubCategoryTests
{
    private readonly Mock<IAccountValidator> _validatorMock;

    public AccountAddTemplateSubCategoryTests()
    {
        _validatorMock = new Mock<IAccountValidator>();
        SetupValidatorToReturnSuccess();
    }

    #region Success Cases

    [Fact]
    public async Task AddTemplateSubCategory_WithValidTemplateSubCategory_ShouldAddSuccessfully()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        var categoryResult = account.AddTemplateCategory(category, ownerUserId);
        Assert.True(categoryResult.IsSuccess, $"Category addition failed: {string.Join(", ", categoryResult.Errors.Select(e => e.Message))}");

        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess, $"SubCategory addition failed: {string.Join(", ", result.Errors.Select(e => e.Message))}");
        _ = Assert.Single(account.SubCategories);
        Assert.Equal(subCategory.Id, account.SubCategories.First().SubCategoryId);
    }

    [Fact]
    public async Task AddTemplateSubCategory_ShouldSetCorrectOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, account.SubCategories.First().Order);
    }

    [Fact]
    public async Task AddTemplateSubCategory_MultipleTimes_ShouldIncrementOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);

        var subCategory1 = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var subCategory2 = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var subCategory3 = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        _ = account.AddTemplateSubCategory(subCategory1, ownerUserId);
        _ = account.AddTemplateSubCategory(subCategory2, ownerUserId);
        _ = account.AddTemplateSubCategory(subCategory3, ownerUserId);

        // Assert
        Assert.Equal(3, account.SubCategories.Count);
        Assert.Equal(1, account.SubCategories.ElementAt(0).Order);
        Assert.Equal(2, account.SubCategories.ElementAt(1).Order);
        Assert.Equal(3, account.SubCategories.ElementAt(2).Order);
    }

    [Fact]
    public async Task AddTemplateSubCategory_ShouldRaiseDomainEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(account.Events, e => e is AccountSubCategoryAddedDomainEvent);

        var domainEvent = account.Events.OfType<AccountSubCategoryAddedDomainEvent>().Last();
        Assert.Equal(ownerUserId, domainEvent.UserId);
    }

    #endregion

    #region Validation - Null SubCategory

    [Fact]
    public async Task AddTemplateSubCategory_WithNullSubCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        SubCategory? subCategory = null;
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateSubCategory(subCategory!, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.SubCategoryCannotBeNull));
        Assert.Empty(account.SubCategories);
    }

    #endregion

    #region Validation - IsTemplate

    [Fact]
    public async Task AddTemplateSubCategory_WithCustomSubCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var owner = AccountId.New();
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);
        var subCategory = SubCategoryFixture.GetCustomSubCategoryWithOwner(category: category, owner: owner);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error == AccountErrorCode.SubCategoryMustBeTemplate);
        Assert.Empty(account.SubCategories);
    }

    #endregion

    #region Validation - Duplicate SubCategory

    [Fact]
    public async Task AddTemplateSubCategory_WithDuplicateSubCategory_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        _ = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error == AccountErrorCode.SubCategoryAlreadyAdded);
        _ = Assert.Single(account.SubCategories);
    }

    #endregion

    #region Validation - Category Not In Account

    [Fact]
    public async Task AddTemplateSubCategory_WithCategoryNotInAccount_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e.Error == AccountErrorCode.SubCategoryCategoryNotInAccount);
        Assert.Empty(account.SubCategories);
    }

    [Fact]
    public async Task AddTemplateSubCategory_WithCategoryNotInAccount_ShouldNotRaiseEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var ownerUserId = account.Members.First().UserId;
        var initialEventCount = account.Events.Count;

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.DoesNotContain(account.Events.Skip(initialEventCount), e => e is AccountSubCategoryAddedDomainEvent);
    }

    #endregion

    #region Permission Validation

    [Fact]
    public async Task AddTemplateSubCategory_WithUserNotMember_ShouldFail()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var nonMemberUserId = Guid.NewGuid();

        // Act
        var result = account.AddTemplateSubCategory(subCategory, nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, n =>
            n.FieldName == "userId" &&
            n.Error == AccountErrorCode.UserNotMember);
        Assert.Empty(account.SubCategories);
    }

    [Fact]
    public async Task AddTemplateSubCategory_WithViewerPermission_ShouldFail()
    {
        // Arrange
        var (account, viewerUserId) = await CreateAccountWithAdditionalMember(PermissionType.Viewer);
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First(m => m.PermissionType == PermissionType.Owner).UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, viewerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, n =>
            n.FieldName == "userId" &&
            n.Error == AccountErrorCode.InsufficientPermissions);
        Assert.Empty(account.SubCategories);
    }

    [Fact]
    public async Task AddTemplateSubCategory_WithEditorPermission_ShouldSucceed()
    {
        // Arrange
        var (account, editorUserId) = await CreateAccountWithAdditionalMember(PermissionType.Editor);
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First(m => m.PermissionType == PermissionType.Owner).UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, editorUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.SubCategories);
        Assert.Equal(subCategory.Id, account.SubCategories.First().SubCategoryId);
    }

    [Fact]
    public async Task AddTemplateSubCategory_WithOwnerPermission_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccount();
        var category = CategoryFixture.GetTemplateCategory();
        var ownerUserId = account.Members.First().UserId;

        _ = account.AddTemplateCategory(category, ownerUserId);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);

        // Act
        var result = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.SubCategories);
        Assert.Equal(subCategory.Id, account.SubCategories.First().SubCategoryId);
    }

    [Fact]
    public async Task AddTemplateSubCategory_WithMultipleValidationErrors_ShouldIncludePermissionError()
    {
        // Arrange
        var account = await CreateValidAccount();
        var nonMemberUserId = Guid.NewGuid();

        // Act - passa subcategoria null E usuário não membro
        var result = account.AddTemplateSubCategory(null!, nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, n =>
            n.FieldName == "userId" &&
            n.Error == AccountErrorCode.UserNotMember);
        Assert.Contains(result.Errors, n =>
            n.FieldName == nameof(SubCategory) &&
            n.Error == AccountErrorCode.SubCategoryCannotBeNull);
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
        // Cria account base
        var account = await CreateValidAccount();

        // Pega o userId do owner para usar no LinkMember
        var ownerUserId = account.Members.First().UserId;

        // Adiciona membro adicional com permissão especificada
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
