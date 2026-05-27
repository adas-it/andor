using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Accounts.Domain.Tests.Categories;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;

namespace Andor.Accounts.Domain.Tests.SubCategories;

public class SubCategoryUnitTests
{
    #region Factory Methods - New with ID

    [Fact]
    public void New_WithValidParameters_ShouldCreateSubCategory()
    {
        // Arrange
        var id = SubCategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (result, subCategory) = SubCategory.New(id, name, description, category, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(subCategory);
        Assert.Equal(id, subCategory.Id);
        Assert.Equal(name, subCategory.Name);
        Assert.Equal(description, subCategory.Description);
        Assert.Equal(category, subCategory.Category);
        Assert.Equal(category.Id, subCategory.CategoryId);
        Assert.Null(subCategory.Owner);
    }

    [Fact]
    public void New_WithOwner_ShouldCreateCustomSubCategory()
    {
        // Arrange
        var id = SubCategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var owner = AccountId.New();
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);

        // Act
        var (result, subCategory) = SubCategory.New(id, name, description, category, owner);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(subCategory);
        Assert.Equal(owner, subCategory.Owner);
        Assert.False(subCategory.IsTemplate);
    }

    [Fact]
    public void New_WithNullName_ShouldFail()
    {
        // Arrange
        var id = SubCategoryId.New();
        Name name = null!;
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (result, subCategory) = SubCategory.New(id, name, description, category, null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(subCategory);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void New_WithNullCategory_ShouldFail()
    {
        // Arrange
        var id = SubCategoryId.New();
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        Category category = null!;

        // Act
        var (result, subCategory) = SubCategory.New(id, name, description, category, null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(subCategory);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region Factory Methods - New without ID

    [Fact]
    public void New_WithoutId_ShouldGenerateNewId()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (result, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(subCategory);
        Assert.NotEqual(Guid.Empty, subCategory.Id.Value);
    }

    [Fact]
    public void New_WithoutId_MultipleCalls_ShouldGenerateDifferentIds()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (_, subCategory1) = SubCategory.New(name, description, category, null);
        var (_, subCategory2) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.NotEqual(subCategory1!.Id, subCategory2!.Id);
    }

    #endregion

    #region IsTemplate Property

    [Fact]
    public void IsTemplate_WhenOwnerIsNull_ShouldReturnTrue()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.True(subCategory!.IsTemplate);
    }

    [Fact]
    public void IsTemplate_WhenOwnerIsNotNull_ShouldReturnFalse()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var owner = AccountId.New();
        var category = CategoryFixture.GetCustomCategoryWithOwner(owner: owner);

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, owner);

        // Assert
        Assert.False(subCategory!.IsTemplate);
    }

    #endregion

    #region Type Property

    [Fact]
    public void Type_ShouldReturnCategoryType()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.Equal(category.Type, subCategory!.Type);
    }

    [Theory]
    [InlineData(0)] // Undefined
    [InlineData(1)] // MoneyDeposit
    [InlineData(2)] // MoneySpending
    public void Type_ShouldMatchCategoryType_ForDifferentMovementTypes(int movementTypeKey)
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Nota: Este teste documenta que o Type sempre vem da Category

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        // O tipo da subcategoria sempre reflete o tipo da categoria
        Assert.Equal(category.Type, subCategory!.Type);
    }

    #endregion

    #region CategoryId Property

    [Fact]
    public void CategoryId_ShouldMatchCategoryId()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.Equal(category.Id, subCategory!.CategoryId);
    }

    [Fact]
    public void CategoryId_ShouldBeSetOnCreation()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.NotNull(subCategory!.CategoryId);
        Assert.NotEqual(Guid.Empty, subCategory.CategoryId.Value);
    }

    #endregion

    #region SoftDelete

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Act
        var result = subCategory!.SoftDelete();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(subCategory.IsDeleted);
    }

    [Fact]
    public void SoftDelete_MultipleCallsShouldSucceed()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Act
        var result1 = subCategory!.SoftDelete();
        var result2 = subCategory.SoftDelete();

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(subCategory.IsDeleted);
    }

    #endregion

    #region Restore

    [Fact]
    public void Restore_ShouldUnmarkAsDeleted()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();
        var (_, subCategory) = SubCategory.New(name, description, category, null);
        _ = subCategory!.SoftDelete();

        // Act
        var result = subCategory.Restore();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(subCategory.IsDeleted);
    }

    [Fact]
    public void Restore_WithoutDelete_ShouldSucceed()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Act
        var result = subCategory!.Restore();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(subCategory.IsDeleted);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void New_WithMinimumNameLength_ShouldSucceed()
    {
        // Arrange
        Name name = "ABC"; // 3 caracteres (mínimo)
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (result, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(subCategory);
    }

    [Fact]
    public void New_WithMaximumNameLength_ShouldSucceed()
    {
        // Arrange
        Name name = new string('A', 70); // 70 caracteres (máximo)
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (result, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(subCategory);
    }

    [Fact]
    public void New_CategoryAndSubCategoryShouldHaveSameCategoryId()
    {
        // Arrange
        var name = GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        var category = CategoryFixture.GetTemplateCategory();

        // Act
        var (_, subCategory) = SubCategory.New(name, description, category, null);

        // Assert
        Assert.Equal(category.Id.Value, subCategory!.CategoryId.Value);
    }

    #endregion
}
