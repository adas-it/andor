using Andor.Domain.Common.ValuesObjects;

namespace Andor.Accounts.Domain.Categories.Errors;

/// <summary>
/// Error messages for Category domain operations.
/// </summary>
public static class CategoryErrorMessages
{
    public const string MovementTypeCannotBeUndefined = "Movement type cannot be undefined.";
    public const string NameCannotBeNull = "Category name cannot be null or empty.";
}

/// <summary>
/// Error codes for Category domain operations.
/// Codes in the 6000-6999 range.
/// </summary>
public sealed record CategoryErrorCode
{
    // Validation errors (6000-6099)
    public static readonly DomainErrorCode MovementTypeCannotBeUndefined = DomainErrorCode.New(6_000);
    public static readonly DomainErrorCode NameCannotBeNull = DomainErrorCode.New(6_001);
}
