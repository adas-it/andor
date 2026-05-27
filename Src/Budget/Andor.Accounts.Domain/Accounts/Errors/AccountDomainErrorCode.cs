using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts.Errors;

public sealed record AccountErrorCode(int original) : DomainErrorCode(original)
{
    public static readonly DomainErrorCode AccountErrorOnDelete = new AccountErrorCode(3_001);
    public static readonly DomainErrorCode AccountShouldHaveOneOwner = new AccountErrorCode(3_002);

    // AddTemplateCategory errors
    public static readonly DomainErrorCode CategoryCannotBeNull = new AccountErrorCode(3_100);
    public static readonly DomainErrorCode CategoryMustBeTemplate = new AccountErrorCode(3_101);
    public static readonly DomainErrorCode CategoryAlreadyAdded = new AccountErrorCode(3_102);
    public static readonly DomainErrorCode CannotAddDeletedCategory = new AccountErrorCode(3_103);
    public static readonly DomainErrorCode UserNotMember = new AccountErrorCode(3_104);
    public static readonly DomainErrorCode InsufficientPermissions = new AccountErrorCode(3_105);

    // AddTemplateSubCategory errors
    public static readonly DomainErrorCode SubCategoryCannotBeNull = new AccountErrorCode(3_200);
    public static readonly DomainErrorCode SubCategoryMustBeTemplate = new AccountErrorCode(3_201);
    public static readonly DomainErrorCode SubCategoryAlreadyAdded = new AccountErrorCode(3_202);
    public static readonly DomainErrorCode SubCategoryCategoryNotInAccount = new AccountErrorCode(3_203);
    public static readonly DomainErrorCode PaymentMethodShouldBeSameTypeAsCategory = new AccountErrorCode(3_204);
    public static readonly DomainErrorCode SubCategoryPaymentMethodNotInAccount = new AccountErrorCode(3_205);

    // AddTemplatePaymentMethod errors
    public static readonly DomainErrorCode PaymentMethodCannotBeNull = new AccountErrorCode(3_300);
    public static readonly DomainErrorCode PaymentMethodMustBeTemplate = new AccountErrorCode(3_301);
    public static readonly DomainErrorCode PaymentMethodAlreadyAdded = new AccountErrorCode(3_302);
    public static readonly DomainErrorCode CannotAddDeletedPaymentMethod = new AccountErrorCode(3_303);

    // LinkMember errors
    public static readonly DomainErrorCode UserCannotBeNull = new AccountErrorCode(3_400);
    public static readonly DomainErrorCode UserAlreadyMember = new AccountErrorCode(3_401);
    public static readonly DomainErrorCode OnlyOwnerCanLinkMembers = new AccountErrorCode(3_402);

    // InviteMember errors
    public static readonly DomainErrorCode InviteCannotBeNull = new AccountErrorCode(3_500);
    public static readonly DomainErrorCode InviteAlreadyExists = new AccountErrorCode(3_501);
    public static readonly DomainErrorCode OnlyOwnerCanInviteMembers = new AccountErrorCode(3_502);
    public static readonly DomainErrorCode CannotInviteExistingMember = new AccountErrorCode(3_503);
    public static readonly DomainErrorCode InviteNotFound = new AccountErrorCode(3_504);
    public static readonly DomainErrorCode UserNotInvited = new AccountErrorCode(3_505);

    // FinancialMovement errors
    public static readonly DomainErrorCode FinancialMovementCannotBeNull = new AccountErrorCode(3_600);
    public static readonly DomainErrorCode FinancialMovementSubCategoryNotInAccount = new AccountErrorCode(3_601);
    public static readonly DomainErrorCode FinancialMovementPaymentMethodNotInAccount = new AccountErrorCode(3_602);
    public static readonly DomainErrorCode FinancialMovementCategoryNotInAccount = new AccountErrorCode(3_603);
    public static readonly DomainErrorCode FinancialMovementPaymentMethodTypeMismatch = new AccountErrorCode(3_604);
}
