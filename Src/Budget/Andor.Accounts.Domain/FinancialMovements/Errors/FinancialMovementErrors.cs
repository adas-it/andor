using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.FinancialMovements.Errors;

/// <summary>
/// Error messages for FinancialMovement domain operations.
/// </summary>
public static class FinancialMovementErrorMessages
{
    // Validation errors
    public const string ValueMustBeGreaterThanZero = "Value must be greater than zero.";
    public const string DateCannotBeTooFarInFuture = "Date cannot be more than 5 years in the future.";

    // Business rule errors (from Account aggregate operations)
    public const string FinancialMovementCannotBeNull = "Financial movement cannot be null.";
    public const string SubCategoryNotInAccount = "The subcategory does not belong to this account.";
    public const string PaymentMethodNotInAccount = "The payment method does not belong to this account.";
    public const string CategoryNotInAccount = "The category does not belong to this account.";
    public const string PaymentMethodTypeMismatch = "The payment method type must match the category type.";
}

/// <summary>
/// Error codes for FinancialMovement domain operations.
/// Codes in the 5000-5099 range.
/// </summary>
public sealed record FinancialMovementErrorCode
{
    public static readonly DomainErrorCode ValueMustBeGreaterThanZero = DomainErrorCode.New(5_000);
    public static readonly DomainErrorCode DateCannotBeTooFarInFuture = DomainErrorCode.New(5_001);

    public static readonly DomainErrorCode FinancialMovementCannotBeNull = DomainErrorCode.New(5_002);
    public static readonly DomainErrorCode SubCategoryNotInAccount = DomainErrorCode.New(5_003);
    public static readonly DomainErrorCode PaymentMethodNotInAccount = DomainErrorCode.New(5_004);
    public static readonly DomainErrorCode CategoryNotInAccount = DomainErrorCode.New(5_005);
    public static readonly DomainErrorCode PaymentMethodTypeMismatch = DomainErrorCode.New(5_006);
}
