namespace Andor.Accounts.Domain.Accounts.Errors;

internal static class AccountErrorMessages
{
    public const string AccountShouldHaveOneOwner = "At least one member must have owner permission.";

    // AddTemplateCategory errors
    public const string CategoryCannotBeNull = "Category cannot be null.";
    public const string CategoryMustBeTemplate = "Only template categories can be added through this method.";
    public const string CategoryAlreadyAdded = "Category already added to this account.";
    public const string CannotAddDeletedCategory = "Cannot add a deleted category.";
    public const string UserNotMember = "User is not a member of this account.";
    public const string InsufficientPermissions = "User does not have sufficient permissions to perform this action. Editor or Owner permission required.";

    // AddTemplateSubCategory errors
    public const string SubCategoryCannotBeNull = "SubCategory cannot be null.";
    public const string SubCategoryMustBeTemplate = "Only template subcategories can be added through this method.";
    public const string SubCategoryAlreadyAdded = "SubCategory already added to this account.";
    public const string SubCategoryCategoryNotInAccount = "The subcategory's category must belong to this account.";
    public const string PaymentMethodShouldBeSameTypeAsCategory = "Default payment method must have the same movement type as the subcategory.";
    public const string SubCategoryPaymentMethodNotInAccount = "The subcategory's default payment method must belong to this account.";

    // AddTemplatePaymentMethod errors
    public const string PaymentMethodCannotBeNull = "PaymentMethod cannot be null.";
    public const string PaymentMethodMustBeTemplate = "Only template payment methods can be added through this method.";
    public const string PaymentMethodAlreadyAdded = "PaymentMethod already added to this account.";
    public const string CannotAddDeletedPaymentMethod = "Cannot add a deleted payment method.";

    // LinkMember errors
    public const string UserCannotBeNull = "User cannot be null.";
    public const string UserAlreadyMember = "User is already a member of this account.";
    public const string OnlyOwnerCanLinkMembers = "Only account owners can add new members.";

    // InviteMember errors
    public const string InviteCannotBeNull = "Invite cannot be null.";
    public const string InviteAlreadyExists = "An invite for this user already exists.";
    public const string OnlyOwnerCanInviteMembers = "Only account owners can invite new members.";
    public const string CannotInviteExistingMember = "Cannot invite a user who is already a member of this account.";
    public const string InviteNotFound = "Invite not found.";
    public const string UserNotInvited = "User is not invited to this account or the invite does not belong to this user.";

    // FinancialMovement errors
    public const string FinancialMovementCannotBeNull = "Financial movement cannot be null.";
    public const string FinancialMovementSubCategoryNotInAccount = "The financial movement's subcategory must belong to this account.";
    public const string FinancialMovementPaymentMethodNotInAccount = "The financial movement's payment method must belong to this account.";
    public const string FinancialMovementCategoryNotInAccount = "The financial movement's category must belong to this account.";
    public const string FinancialMovementPaymentMethodTypeMismatch = "The payment method type must match the category type.";
}
