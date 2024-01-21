namespace Family.Budget.Application.Dto.SubCategories.Errors;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
public sealed record SubCategoryErrorCodes : ErrorCode
{
    private SubCategoryErrorCodes(int original) : base(original)
    {
    }

    public static readonly SubCategoryErrorCodes NotFound = new(16_000);
    public static readonly SubCategoryErrorCodes CategoryNotFound = new(16_001);
}

public static class Errors
{
    public static ErrorModel SubCategoryNotFound() => new(SubCategoryErrorCodes.NotFound, "SubCategory Not Found.");
    public static ErrorModel CategoryNotFound() => new(SubCategoryErrorCodes.CategoryNotFound, "Category Not Found.");
}
