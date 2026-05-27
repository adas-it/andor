using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Tests.Currencies;
using Andor.Accounts.Domain.Tests.PaymentMethods;
using Andor.Accounts.Domain.Users;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class AccountAddTemplatePaymentMethodTests
{
    private readonly Mock<IAccountValidator> _validatorMock;

    public AccountAddTemplatePaymentMethodTests()
    {
        _validatorMock = new Mock<IAccountValidator>();
        SetupValidatorToReturnSuccess();
    }

    #region Success Cases

    [Fact]
    public async Task AddTemplatePaymentMethod_WithValidTemplatePaymentMethod_ShouldAddSuccessfully()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.PaymentMethods);
        Assert.Equal(paymentMethod.Id, account.PaymentMethods.First().PaymentMethodId);
    }

    [Fact]
    public async Task AddTemplatePaymentMethod_ShouldSetCorrectOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, account.PaymentMethods.First().Order);
    }

    [Fact]
    public async Task AddTemplatePaymentMethod_MultipleTimes_ShouldIncrementOrder()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod1 = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Credit Card");
        var paymentMethod2 = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Debit Card");
        var paymentMethod3 = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Cash");
        var ownerUserId = account.Members.First().UserId;

        // Act
        _ = account.AddTemplatePaymentMethod(paymentMethod1, ownerUserId);
        _ = account.AddTemplatePaymentMethod(paymentMethod2, ownerUserId);
        _ = account.AddTemplatePaymentMethod(paymentMethod3, ownerUserId);

        // Assert
        Assert.Equal(3, account.PaymentMethods.Count);
        Assert.Equal(1, account.PaymentMethods.ElementAt(0).Order);
        Assert.Equal(2, account.PaymentMethods.ElementAt(1).Order);
        Assert.Equal(3, account.PaymentMethods.ElementAt(2).Order);
    }

    [Fact]
    public async Task AddTemplatePaymentMethod_ShouldRaiseDomainEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(account.Events, e => e is AccountPaymentMethodAddedDomainEvent);

        var domainEvent = account.Events.OfType<AccountPaymentMethodAddedDomainEvent>().First();
        Assert.Equal(ownerUserId, domainEvent.UserId);
    }

    [Fact]
    public async Task AddTemplatePaymentMethod_WithEditorPermission_ShouldSucceed()
    {
        // Arrange
        var (account, editorUserId) = await CreateAccountWithAdditionalMember(PermissionType.Editor);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, editorUserId);

        // Assert
        Assert.True(result.IsSuccess);
        _ = Assert.Single(account.PaymentMethods);
    }

    #endregion

    #region Validation - Null PaymentMethod

    [Fact]
    public async Task AddTemplatePaymentMethod_WithNullPaymentMethod_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        PaymentMethod? paymentMethod = null;
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod!, ownerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.PaymentMethodCannotBeNull));
        Assert.Empty(account.PaymentMethods);
    }

    [Fact]
    public async Task AddTemplatePaymentMethod_WithNullPaymentMethod_ShouldNotRaiseEvent()
    {
        // Arrange
        var account = await CreateValidAccount();
        PaymentMethod? paymentMethod = null;
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod!, ownerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.DoesNotContain(account.Events, e => e is AccountPaymentMethodAddedDomainEvent);
    }

    #endregion

    #region Validation - IsTemplate

    [Fact]
    public async Task AddTemplatePaymentMethod_WithCustomPaymentMethod_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var owner = AccountId.New();
        var paymentMethod = PaymentMethodFixture.GetCustomPaymentMethodWithOwner(owner: owner);
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error == AccountErrorCode.PaymentMethodMustBeTemplate);
        Assert.Empty(account.PaymentMethods);
    }

    #endregion

    #region Validation - Duplicate PaymentMethod

    [Fact]
    public async Task AddTemplatePaymentMethod_WithDuplicatePaymentMethod_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();
        var ownerUserId = account.Members.First().UserId;

        // First add
        _ = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Act - Try to add same payment method again
        var result = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.PaymentMethodAlreadyAdded));
        _ = Assert.Single(account.PaymentMethods);
    }

    #endregion

    #region Validation - Deleted PaymentMethod

    [Fact]
    public async Task AddTemplatePaymentMethod_WithDeletedPaymentMethod_ShouldReturnFailure()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();
        _ = paymentMethod.SoftDelete();
        var ownerUserId = account.Members.First().UserId;

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.CannotAddDeletedPaymentMethod));
        Assert.Empty(account.PaymentMethods);
    }

    #endregion

    #region Permission Tests

    [Fact]
    public async Task AddTemplatePaymentMethod_WithViewerPermission_ShouldFail()
    {
        // Arrange
        var (account, viewerUserId) = await CreateAccountWithAdditionalMember(PermissionType.Viewer);
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, viewerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.InsufficientPermissions));
    }

    [Fact]
    public async Task AddTemplatePaymentMethod_WithUserNotMember_ShouldFail()
    {
        // Arrange
        var account = await CreateValidAccount();
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod();
        var nonMemberUserId = Guid.NewGuid();

        // Act
        var result = account.AddTemplatePaymentMethod(paymentMethod, nonMemberUserId);

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
