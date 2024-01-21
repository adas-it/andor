namespace Family.Budget.Application.Dto.Categories.Errors;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
public sealed record CategoryErrorCodes : ErrorCode
{
    private CategoryErrorCodes(int original) : base(original)
    {
    }

    public static readonly CategoryErrorCodes NotFound = new(12_000);
}

public static class Errors
{
    public static ErrorModel CategoryNotFound() => new(CategoryErrorCodes.NotFound, "Category Not Found.");
}
