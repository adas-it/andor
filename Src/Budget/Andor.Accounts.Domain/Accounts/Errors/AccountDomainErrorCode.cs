using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts.Errors;

public sealed record AccountErrorCode
{
    public static readonly DomainErrorCode AccountErrorOnDelete = DomainErrorCode.New(3_001);
    public static readonly DomainErrorCode AccountShouldHaveOneOwner = DomainErrorCode.New(3_002);
    public static readonly DomainErrorCode CurrencyNotFound = DomainErrorCode.New(3_003);

    // AddTemplateCategory errors
    public static readonly DomainErrorCode CategoryCannotBeNull = DomainErrorCode.New(3_100);
    public static readonly DomainErrorCode CategoryMustBeTemplate = DomainErrorCode.New(3_101);
    public static readonly DomainErrorCode CategoryAlreadyAdded = DomainErrorCode.New(3_102);
    public static readonly DomainErrorCode CannotAddDeletedCategory = DomainErrorCode.New(3_103);
    public static readonly DomainErrorCode UserNotMember = DomainErrorCode.New(3_104);
    public static readonly DomainErrorCode InsufficientPermissions = DomainErrorCode.New(3_105);

    // AddTemplateSubCategory errors
    public static readonly DomainErrorCode SubCategoryCannotBeNull = DomainErrorCode.New(3_200);
    public static readonly DomainErrorCode SubCategoryMustBeTemplate = DomainErrorCode.New(3_201);
    public static readonly DomainErrorCode SubCategoryAlreadyAdded = DomainErrorCode.New(3_202);
    public static readonly DomainErrorCode SubCategoryCategoryNotInAccount = DomainErrorCode.New(3_203);
    public static readonly DomainErrorCode PaymentMethodShouldBeSameTypeAsCategory = DomainErrorCode.New(3_204);
    public static readonly DomainErrorCode SubCategoryPaymentMethodNotInAccount = DomainErrorCode.New(3_205);

    // AddTemplatePaymentMethod errors
    public static readonly DomainErrorCode PaymentMethodCannotBeNull = DomainErrorCode.New(3_300);
    public static readonly DomainErrorCode PaymentMethodMustBeTemplate = DomainErrorCode.New(3_301);
    public static readonly DomainErrorCode PaymentMethodAlreadyAdded = DomainErrorCode.New(3_302);
    public static readonly DomainErrorCode CannotAddDeletedPaymentMethod = DomainErrorCode.New(3_303);

    // LinkMember errors
    public static readonly DomainErrorCode UserCannotBeNull = DomainErrorCode.New(3_400);
    public static readonly DomainErrorCode UserAlreadyMember = DomainErrorCode.New(3_401);
    public static readonly DomainErrorCode OnlyOwnerCanLinkMembers = DomainErrorCode.New(3_402);

    // InviteMember errors
    public static readonly DomainErrorCode InviteCannotBeNull = DomainErrorCode.New(3_500);
    public static readonly DomainErrorCode InviteAlreadyExists = DomainErrorCode.New(3_501);
    public static readonly DomainErrorCode OnlyOwnerCanInviteMembers = DomainErrorCode.New(3_502);
    public static readonly DomainErrorCode CannotInviteExistingMember = DomainErrorCode.New(3_503);
    public static readonly DomainErrorCode InviteNotFound = DomainErrorCode.New(3_504);
    public static readonly DomainErrorCode UserNotInvited = DomainErrorCode.New(3_505);

    // FinancialMovement errors
    public static readonly DomainErrorCode FinancialMovementCannotBeNull = DomainErrorCode.New(3_600);
    public static readonly DomainErrorCode FinancialMovementSubCategoryNotInAccount = DomainErrorCode.New(3_601);
    public static readonly DomainErrorCode FinancialMovementPaymentMethodNotInAccount = DomainErrorCode.New(3_602);
    public static readonly DomainErrorCode FinancialMovementCategoryNotInAccount = DomainErrorCode.New(3_603);
    public static readonly DomainErrorCode FinancialMovementPaymentMethodTypeMismatch = DomainErrorCode.New(3_604);
    public static readonly DomainErrorCode FinancialMovementSubCategoryNotFound = DomainErrorCode.New(3_605);
    public static readonly DomainErrorCode FinancialMovementPaymentMethodNotFound = DomainErrorCode.New(3_606);
}
