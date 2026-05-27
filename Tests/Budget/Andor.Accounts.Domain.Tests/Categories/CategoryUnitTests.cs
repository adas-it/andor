using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.Categories.Errors;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;

namespace Andor.Accounts.Domain.Tests.Categories;

public class CategoryUnitTests
{
    #region Factory Method Tests - New with Id

    [Fact]
    public void New_WithValidIdAndData_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = CategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(id, name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(id, category.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
        Assert.Null(category.Owner);
    }

    [Fact]
    public void New_WithValidIdAndOwner_ShouldCreateCustomCategory()
    {
        // Arrange
        var id = CategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var owner = AccountId.New();

        // Act
        var (result, category) = Category.New(id, name, description, MovementType.MoneySpending, owner);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(owner, category.Owner);
        Assert.False(category.IsTemplate);
    }

    [Fact]
    public void New_WithInvalidName_ShouldReturnFailure()
    {
        // Arrange
        var id = CategoryId.New();
        Name? invalidName = null;
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(id, invalidName!, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(category);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region Factory Method Tests - New without Id (Auto-generate)

    [Fact]
    public void New_WithoutId_ShouldGenerateNewId()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.NotEqual(Guid.Empty, category.Id.Value);
    }

    [Fact]
    public void New_WithoutId_TwoCalls_ShouldGenerateDifferentIds()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (_, category1) = Category.New(name, description, MovementType.MoneySpending, null);
        var (_, category2) = Category.New(name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.NotEqual(category1!.Id, category2!.Id);
    }

    [Fact]
    public void New_WithoutIdButWithOwner_ShouldCreateCustomCategory()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var owner = AccountId.New();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneySpending, owner);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(owner, category.Owner);
        Assert.False(category.IsTemplate);
    }

    #endregion

    #region IsTemplate Tests

    [Fact]
    public void IsTemplate_WhenOwnerIsNull_ShouldReturnTrue()
    {
        // Arrange
        var category = CategoryFixture.GetTemplateCategory();

        // Act & Assert
        Assert.True(category.IsTemplate);
        Assert.Null(category.Owner);
    }

    [Fact]
    public void IsTemplate_WhenOwnerIsSet_ShouldReturnFalse()
    {
        // Arrange
        var owner = AccountId.New();
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);

        // Act & Assert
        Assert.False(category.IsTemplate);
        _ = Assert.NotNull(category.Owner);
        Assert.Equal(owner, category.Owner);
    }

    #endregion

    #region Soft Delete Tests

    [Fact]
    public void SoftDelete_ShouldMarkCategoryAsDeleted()
    {
        // Arrange
        var category = CategoryFixture.GetTemplateCategory();
        Assert.False(category.IsDeleted);

        // Act
        var result = category.SoftDelete();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(category.IsDeleted);
    }

    [Fact]
    public void SoftDelete_CalledTwice_ShouldStayDeleted()
    {
        // Arrange
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        _ = category.SoftDelete();
        var result = category.SoftDelete();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(category.IsDeleted);
    }

    #endregion

    #region Restore Tests

    [Fact]
    public void Restore_OnDeletedCategory_ShouldMarkAsNotDeleted()
    {
        // Arrange
        var category = CategoryFixture.GetTemplateCategory();
        _ = category.SoftDelete();
        Assert.True(category.IsDeleted);

        // Act
        var result = category.Restore();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(category.IsDeleted);
    }

    [Fact]
    public void Restore_OnNonDeletedCategory_ShouldStayNotDeleted()
    {
        // Arrange
        var category = CategoryFixture.GetTemplateCategory();
        Assert.False(category.IsDeleted);

        // Act
        var result = category.Restore();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(category.IsDeleted);
    }

    [Fact]
    public void Restore_AfterSoftDelete_ShouldAllowToggle()
    {
        // Arrange
        var category = CategoryFixture.GetTemplateCategory();

        // Act & Assert - Delete
        _ = category.SoftDelete();
        Assert.True(category.IsDeleted);

        // Act & Assert - Restore
        _ = category.Restore();
        Assert.False(category.IsDeleted);

        // Act & Assert - Delete again
        _ = category.SoftDelete();
        Assert.True(category.IsDeleted);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void New_WithNullName_ShouldReturnFailure()
    {
        // Arrange
        Name? name = null;
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name!, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(category);
        Assert.Contains(result.Errors, e => e.FieldName == nameof(Name));
    }

    [Fact]
    public void New_WithUndefinedMovementType_ShouldReturnFailure()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.Undefined, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(category);
        Assert.Contains(result.Errors, e =>
            e.FieldName == nameof(Category.Type) &&
            e.Error.Equals(CategoryErrorCode.MovementTypeCannotBeUndefined));
    }

    [Fact]
    public void New_WithValidMovementType_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(MovementType.MoneySpending, category.Type);
    }

    [Fact]
    public void New_WithMoneySpendingType_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(MovementType.MoneySpending, category.Type);
    }

    [Fact]
    public void New_WithMoneyDepositType_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneyDeposit, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(MovementType.MoneyDeposit, category.Type);
    }

    [Fact]
    public void Category_ShouldStoreMovementTypeCorrectly()
    {
        // Arrange
        var id = CategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var expectedType = MovementType.MoneyDeposit;

        // Act
        var (_, category) = Category.New(id, name, description, expectedType, null);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(expectedType, category.Type);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Category_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var category = CategoryFixture.GetTemplateCategory();

        // Assert
        Assert.NotNull(category.SubCategories);
        Assert.Empty(category.SubCategories);
        Assert.False(category.IsDeleted);
    }

    [Fact]
    public void Category_Properties_ShouldBeSetCorrectly()
    {
        // Arrange
        var id = CategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var owner = AccountId.New();

        // Act
        var (_, category) = Category.New(id, name, description, MovementType.MoneySpending, owner);

        // Assert
        Assert.Equal(id, category!.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
        Assert.Equal(owner, category.Owner);
        Assert.NotNull(category.SubCategories);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void New_WithMinimumValidName_ShouldCreateSuccessfully()
    {
        // Arrange
        Name name = "ABC"; // Minimum 3 characters
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal("ABC", category.Name.Value);
    }

    [Fact]
    public void New_WithMaximumValidName_ShouldCreateSuccessfully()
    {
        // Arrange
        var maxName = new string('A', Name.MaxLength);
        Name name = maxName;
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (result, category) = Category.New(name, description, MovementType.MoneySpending, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal(maxName, category.Name.Value);
    }

    [Fact]
    public void Category_IdShouldBeUnique()
    {
        // Arrange & Act
        var category1 = CategoryFixture.GetTemplateCategory();
        var category2 = CategoryFixture.GetTemplateCategory();

        // Assert
        Assert.NotEqual(category1.Id, category2.Id);
    }

    #endregion

    #region Template vs Custom Category Tests

    [Fact]
    public void TemplateCategory_ShouldHaveNullOwnerAndIsTemplateTrue()
    {
        // Arrange & Act
        var category = CategoryFixture.GetTemplateCategory();

        // Assert
        Assert.Null(category.Owner);
        Assert.True(category.IsTemplate);
    }

    [Fact]
    public void CustomCategory_ShouldHaveOwnerAndIsTemplateFalse()
    {
        // Arrange
        var owner = AccountId.New();

        // Act
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);

        // Assert
        _ = Assert.NotNull(category.Owner);
        Assert.Equal(owner, category.Owner);
        Assert.False(category.IsTemplate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Category_WithDifferentOwnerStates_ShouldHaveCorrectIsTemplate(bool hasOwner)
    {
        // Arrange
        AccountId? owner = hasOwner ? AccountId.New() : null;
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();

        // Act
        var (_, category) = Category.New(name, description, MovementType.MoneySpending, owner);

        // Assert
        Assert.Equal(!hasOwner, category!.IsTemplate);
    }

    #endregion
}
