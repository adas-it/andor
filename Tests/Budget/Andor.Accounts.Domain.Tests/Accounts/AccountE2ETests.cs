using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Tests.Categories;
using Andor.Accounts.Domain.Tests.FinancialMovements;
using Andor.Accounts.Domain.Tests.PaymentMethods;
using Andor.Accounts.Domain.Tests.SubCategories;
using Andor.Accounts.Domain.Users;
using Andor.Accounts.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Tests.Accounts;

/// <summary>
/// End-to-End tests for the Account domain aggregate demonstrating complete workflows.
/// These tests validate the happy path scenarios for creating accounts and managing their entities.
/// </summary>
public class AccountE2ETests
{

    #region E2E - Happy Path with Template Entities

    [Fact]
    public async Task E2E_HappyPath_CreateAccount_AddTemplateEntities()
    {
        // Step 1: Create Account
        var accountId = AccountId.New();
        var ownerUserId = Guid.NewGuid();

        var (createResult, account) =
            await AccountFixture.CreateValidAccountAsync(accountId: accountId, ownerUserId: ownerUserId);

        Assert.True(createResult.IsSuccess);
        Assert.NotNull(account);
        Assert.Equal(accountId, account!.Id);
        _ = Assert.Single(account.Members);
        Assert.Equal(PermissionType.Owner, account.Members.First().PermissionType);

        // Step 2: Add Template Payment Method
        var templatePaymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Credit Card");
        var addPaymentMethodResult = account.AddTemplatePaymentMethod(templatePaymentMethod, ownerUserId);
        Assert.True(addPaymentMethodResult.IsSuccess);
        _ = Assert.Single(account.PaymentMethods);

        // Step 3: Add Template Category
        var templateCategory = CategoryFixture.GetTemplateCategory(name: "Food");
        var addCategoryResult = account.AddTemplateCategory(templateCategory, ownerUserId);
        Assert.True(addCategoryResult.IsSuccess);
        _ = Assert.Single(account.Categories);

        // Step 4: Add Template SubCategory
        var templateSubCategory = SubCategoryFixture.GetTemplateSubCategory(
            name: "Groceries",
            category: templateCategory);
        var addSubCategoryResult = account.AddTemplateSubCategory(templateSubCategory, ownerUserId);
        Assert.True(addSubCategoryResult.IsSuccess);
        _ = Assert.Single(account.SubCategories);

        // Step 5: Add Financial Movement
        var financialMovement = FinancialMovementFixture.CreateFinancialMovement(
            date: DateTime.UtcNow,
            description: "Grocery shopping",
            subCategory: templateSubCategory,
            paymentMethod: templatePaymentMethod,
            account: account,
            value: 150.00m);

        var addMovementResult = account.AddFinancialMovement(financialMovement, ownerUserId);
        Assert.True(addMovementResult.IsSuccess);

        // Verify Financial Movement Event was raised
        Assert.Contains(account.Events, e => e is AccountFinancialMovementAddedDomainEvent);
        var movementEvent = account.Events.OfType<AccountFinancialMovementAddedDomainEvent>().First();
        Assert.Equal(ownerUserId, movementEvent.UserId);

        // Final Validation
        Assert.Equal(1, account.Categories.Count);
        Assert.Equal(1, account.SubCategories.Count);
        Assert.Equal(1, account.PaymentMethods.Count);
        Assert.Equal(templateCategory.Id, account.SubCategories.First().SubCategory.CategoryId);
    }

    #endregion

    #region E2E - Happy Path with Custom Entities

    [Fact]
    public async Task E2E_HappyPath_CreateAccount_CreateCustomEntities()
    {
        // Step 1: Create Account
        var (createResult, account) = await CreateValidAccount();
        Assert.True(createResult.IsSuccess);
        var ownerUserId = account!.Members.First().UserId;

        // Step 2: Create Custom Payment Method
        var paymentMethodResult = account.CreateCustomPaymentMethod(
            "Debit Card",
            "My debit card",
            MovementType.MoneySpending,
            ownerUserId);
        Assert.True(paymentMethodResult.IsSuccess);
        var customPaymentMethod = account.PaymentMethods.First().PaymentMethod;

        // Step 3: Create Custom Category
        var categoryResult = account.CreateCustomCategory(
            "Entertainment",
            "Entertainment expenses",
            MovementType.MoneySpending,
            ownerUserId);
        Assert.True(categoryResult.IsSuccess);
        var customCategory = account.Categories.First().Category;

        // Step 4: Create Custom SubCategory
        var subCategoryResult = account.CreateCustomSubCategory(
            "Cinema",
            "Movies",
            customCategory,
            customPaymentMethod,
            ownerUserId);
        Assert.True(subCategoryResult.IsSuccess);
        var customSubCategory = account.SubCategories.First().SubCategory;

        // Step 5: Add Financial Movement
        var financialMovement = FinancialMovementFixture.CreateFinancialMovement(
            date: DateTime.UtcNow,
            description: "Movie tickets",
            subCategory: customSubCategory,
            paymentMethod: customPaymentMethod,
            account: account,
            value: 50.00m);

        var addMovementResult = account.AddFinancialMovement(financialMovement, ownerUserId);
        Assert.True(addMovementResult.IsSuccess);

        // Verify Financial Movement Event was raised
        Assert.Contains(account.Events, e => e is AccountFinancialMovementAddedDomainEvent);
        var movementEvent = account.Events.OfType<AccountFinancialMovementAddedDomainEvent>().First();
        Assert.Equal(ownerUserId, movementEvent.UserId);

        // Final Validation
        Assert.Equal(1, account.Categories.Count);
        Assert.Equal(1, account.SubCategories.Count);
        Assert.Equal(1, account.PaymentMethods.Count);
        Assert.False(account.Categories.First().Category.IsTemplate);
        Assert.False(account.SubCategories.First().SubCategory.IsTemplate);
        Assert.False(account.PaymentMethods.First().PaymentMethod.IsTemplate);
    }

    #endregion

    #region E2E - Member Invite Flow

    [Fact]
    public async Task E2E_InviteFlow_InviteByEmail_LinkUser_AddMember()
    {
        // Step 1: Create Account
        var (createResult, account) = await CreateValidAccount();
        var ownerUserId = account!.Members.First().UserId;

        // Step 2: Invite Member by Email
        var inviteEmail = Email.Create("newuser@example.com");
        var inviteResult = account.InviteMemberByEmail(inviteEmail, PermissionType.Editor, ownerUserId);
        Assert.True(inviteResult.IsSuccess);
        _ = Assert.Single(account.Invites);
        Assert.True(account.Invites.First().IsActive);

        // Step 3: Link UserId to Invite
        var newUserId = UserId.New();
        var linkUserResult = account.LinkUserToInvite(inviteEmail, newUserId, ownerUserId);
        Assert.True(linkUserResult.IsSuccess);
        Assert.Equal(newUserId, account.Invites.First().UserId!.Value);

        // Step 4: User Accepts Invite
        var inviteId = account.Invites.First().Id;
        var respondResult = account.RespondInvite(inviteId, newUserId.Value);
        Assert.True(respondResult.IsSuccess);
        Assert.True(account.Invites.First().IsAccepted);

        // Step 5: Link Member
        var newUser = new User { Id = newUserId.Value };
        var linkMemberResult = account.LinkMember(newUser, PermissionType.Editor, ownerUserId);
        Assert.True(linkMemberResult.IsSuccess);
        Assert.Equal(2, account.Members.Count);

        var editorMember = account.Members.First(m => m.UserId == newUserId.Value);
        Assert.Equal(PermissionType.Editor, editorMember.PermissionType);

        // Step 6: Setup entities for financial movement
        var paymentMethod = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Cash");
        _ = account.AddTemplatePaymentMethod(paymentMethod, ownerUserId);

        var category = CategoryFixture.GetTemplateCategory(name: "Utilities");
        _ = account.AddTemplateCategory(category, ownerUserId);

        var subCategory = SubCategoryFixture.GetTemplateSubCategory(name: "Electricity", category: category);
        _ = account.AddTemplateSubCategory(subCategory, ownerUserId);

        // Step 7: New member creates a financial movement
        var financialMovement = FinancialMovementFixture.CreateFinancialMovement(
            date: DateTime.UtcNow,
            description: "Electricity bill payment",
            subCategory: subCategory,
            paymentMethod: paymentMethod,
            account: account,
            value: 200.00m);

        var addMovementResult = account.AddFinancialMovement(financialMovement, newUserId.Value);
        Assert.True(addMovementResult.IsSuccess);

        // Verify Financial Movement Event was raised with new editor's UserId
        Assert.Contains(account.Events, e => e is AccountFinancialMovementAddedDomainEvent);
        var movementEvent = account.Events.OfType<AccountFinancialMovementAddedDomainEvent>().Last();
        Assert.Equal(newUserId.Value, movementEvent.UserId);
    }

    #endregion

    #region E2E - Complex Scenario

    [Fact]
    public async Task E2E_ComplexScenario_MultipleMembers_MixedEntities()
    {
        // Step 1: Create Account
        var (createResult, account) = await CreateValidAccount();
        var ownerUserId = account!.Members.First().UserId;

        // Step 2: Add Template Entities
        var cashPayment = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Cash");
        var creditCardPayment = PaymentMethodFixture.GetTemplatePaymentMethod(name: "Credit Card");
        _ = account.AddTemplatePaymentMethod(cashPayment, ownerUserId);
        _ = account.AddTemplatePaymentMethod(creditCardPayment, ownerUserId);

        var foodCategory = CategoryFixture.GetTemplateCategory(name: "Food");
        _ = account.AddTemplateCategory(foodCategory, ownerUserId);

        var groceriesSubCategory = SubCategoryFixture.GetTemplateSubCategory(
            name: "Groceries", category: foodCategory);
        _ = account.AddTemplateSubCategory(groceriesSubCategory, ownerUserId);

        // Step 3: Create Custom Entities
        _ = account.CreateCustomCategory("Personal", "Personal expenses", MovementType.MoneySpending, ownerUserId);
        var personalCategory = account.Categories.Last().Category;

        _ = account.CreateCustomSubCategory(
            "Hobbies", "Hobbies",
            personalCategory, creditCardPayment, ownerUserId);

        // Step 4: Add Members
        var secondOwner = new User { Id = Guid.NewGuid() };
        _ = account.LinkMember(secondOwner, PermissionType.Owner, ownerUserId);

        var editor = new User { Id = Guid.NewGuid() };
        _ = account.LinkMember(editor, PermissionType.Editor, ownerUserId);

        // Step 5: Add Financial Movements by different members
        var hobbiesSubCategory = account.SubCategories.Last().SubCategory;

        // Owner creates a financial movement
        var ownerMovement = FinancialMovementFixture.CreateFinancialMovement(
            date: DateTime.UtcNow.AddDays(-2),
            description: "Weekly groceries",
            subCategory: groceriesSubCategory,
            paymentMethod: cashPayment,
            account: account,
            value: 120.00m);

        var ownerMovementResult = account.AddFinancialMovement(ownerMovement, ownerUserId);
        Assert.True(ownerMovementResult.IsSuccess);

        // Editor creates a financial movement
        var editorMovement = FinancialMovementFixture.CreateFinancialMovement(
            date: DateTime.UtcNow.AddDays(-1),
            description: "Books and magazines",
            subCategory: hobbiesSubCategory,
            paymentMethod: creditCardPayment,
            account: account,
            value: 75.50m);

        var editorMovementResult = account.AddFinancialMovement(editorMovement, editor.Id);
        Assert.True(editorMovementResult.IsSuccess);

        // Verify both Financial Movement Events were raised
        var movementEvents = account.Events.OfType<AccountFinancialMovementAddedDomainEvent>().ToList();
        Assert.Equal(2, movementEvents.Count);
        Assert.Contains(movementEvents, e => e.UserId == ownerUserId);
        Assert.Contains(movementEvents, e => e.UserId.Equals(editor.Id));

        // Final Validation
        Assert.Equal(2, account.Categories.Count);
        Assert.Equal(2, account.SubCategories.Count);
        Assert.Equal(2, account.PaymentMethods.Count);
        Assert.Equal(3, account.Members.Count);
        Assert.Equal(2, account.Members.Count(m => m.PermissionType == PermissionType.Owner));
        _ = Assert.Single(account.Members.Where(m => m.PermissionType == PermissionType.Editor));
    }

    #endregion

    #region Helper Methods

    private async Task<(DomainResult, Account?)> CreateValidAccount()
        => await AccountFixture.CreateValidAccountAsync();

    #endregion
}
