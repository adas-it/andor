using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.PaymentMethods.Errors;

/// <summary>
/// Error messages for PaymentMethod domain operations.
/// </summary>
public static class PaymentMethodErrorMessages
{
    public const string MovementTypeCannotBeUndefined = "Movement type cannot be undefined.";
    public const string NameCannotBeNull = "Payment method name cannot be null or empty.";
}

/// <summary>
/// Error codes for PaymentMethod domain operations.
/// Codes in the 7000-7999 range.
/// </summary>
public sealed record PaymentMethodErrorCode
{
    // Validation errors (7000-7099)
    public static readonly DomainErrorCode MovementTypeCannotBeUndefined = DomainErrorCode.New(7_000);
    public static readonly DomainErrorCode NameCannotBeNull = DomainErrorCode.New(7_001);
}
