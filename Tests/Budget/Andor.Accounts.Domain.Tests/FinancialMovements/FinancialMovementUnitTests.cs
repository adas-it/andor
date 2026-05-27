using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.Errors;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.Tests.Accounts;
using Andor.Accounts.Domain.Tests.Categories;
using Andor.Accounts.Domain.Tests.PaymentMethods;
using Andor.Accounts.Domain.Tests.SubCategories;

namespace Andor.Accounts.Domain.Tests.FinancialMovements;

public class FinancialMovementUnitTests
{
    #region Payment Method Type Validation Tests

    [Fact]
    public async Task New_WithMatchingPaymentMethodType_ShouldCreateSuccessfully()
    {
        // Arrange
        var account = await CreateValidAccountAsync();
        var category = CategoryFixture.GetTemplateCategory(type: MovementType.MoneySpending);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(type: MovementType.MoneySpending);
        var date = DateTime.UtcNow;
        var value = 100.50m;

        // Act
        var (result, movement) = FinancialMovement.New(
            date,
            "Test movement",
            subCategory,
            paymentMethod,
            account,
            value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(movement);
        Assert.Equal(MovementType.MoneySpending, movement.Type);
        Assert.Equal(paymentMethod.Type, movement.PaymentMethod.Type);
    }

    [Fact]
    public async Task New_WithMismatchedPaymentMethodType_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccountAsync();
        var category = CategoryFixture.GetTemplateCategory(type: MovementType.MoneySpending);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(type: MovementType.MoneyDeposit);
        var date = DateTime.UtcNow;
        var value = 100.50m;

        // Act
        var (result, movement) = FinancialMovement.New(
            date,
            "Test movement",
            subCategory,
            paymentMethod,
            account,
            value);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(movement);
        Assert.Contains(result.Errors, e =>
            e.FieldName == nameof(FinancialMovement.PaymentMethod) &&
            e.Error.Equals(FinancialMovementErrorCode.PaymentMethodTypeMismatch));
    }

    [Fact]
    public async Task New_MoneySpendingCategory_WithMoneySpendingPaymentMethod_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccountAsync();
        var category = CategoryFixture.GetTemplateCategory(type: MovementType.MoneySpending);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(type: MovementType.MoneySpending);

        // Act
        var (result, movement) = FinancialMovement.New(
            DateTime.UtcNow,
            "Groceries",
            subCategory,
            paymentMethod,
            account,
            150.00m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(movement);
        Assert.Equal(MovementType.MoneySpending, movement.Type);
    }

    [Fact]
    public async Task New_MoneyDepositCategory_WithMoneyDepositPaymentMethod_ShouldSucceed()
    {
        // Arrange
        var account = await CreateValidAccountAsync();
        var category = CategoryFixture.GetTemplateCategory(type: MovementType.MoneyDeposit);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(type: MovementType.MoneyDeposit);

        // Act
        var (result, movement) = FinancialMovement.New(
            DateTime.UtcNow,
            "Salary",
            subCategory,
            paymentMethod,
            account,
            5000.00m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(movement);
        Assert.Equal(MovementType.MoneyDeposit, movement.Type);
    }

    [Fact]
    public async Task New_MoneySpendingCategory_WithMoneyDepositPaymentMethod_ShouldFail()
    {
        // Arrange
        var account = await CreateValidAccountAsync();
        var category = CategoryFixture.GetTemplateCategory(type: MovementType.MoneySpending);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(type: MovementType.MoneyDeposit);

        // Act
        var (result, movement) = FinancialMovement.New(
            DateTime.UtcNow,
            "Invalid movement",
            subCategory,
            paymentMethod,
            account,
            100.00m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(movement);
        Assert.Contains(result.Errors, e => e.Error.Equals(FinancialMovementErrorCode.PaymentMethodTypeMismatch));
    }

    [Fact]
    public async Task New_MoneyDepositCategory_WithMoneySpendingPaymentMethod_ShouldFail()
    {
        // Arrange
        var account = await CreateValidAccountAsync();
        var category = CategoryFixture.GetTemplateCategory(type: MovementType.MoneyDeposit);
        var subCategory = SubCategoryFixture.GetTemplateSubCategory(category: category);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(type: MovementType.MoneySpending);

        // Act
        var (result, movement) = FinancialMovement.New(
            DateTime.UtcNow,
            "Invalid deposit",
            subCategory,
            paymentMethod,
            account,
            1000.00m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(movement);
        Assert.Contains(result.Errors, e => e.Error.Equals(FinancialMovementErrorCode.PaymentMethodTypeMismatch));
    }

    #endregion

    #region Helper Methods

    private async Task<Andor.Accounts.Domain.Accounts.Account> CreateValidAccountAsync()
    {
        var (_, account) = await AccountFixture.CreateValidAccountAsync();
        return account!;
    }

    #endregion
}
