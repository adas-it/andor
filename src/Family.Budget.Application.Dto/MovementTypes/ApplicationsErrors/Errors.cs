namespace Family.Budget.Application.Dto.MovementTypes.ApplicationsErrors;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

public sealed record FinancialMovementTypeErrorCodes : ErrorCode
{
    private FinancialMovementTypeErrorCodes(int original) : base(original)
    {
    }

    public static readonly FinancialMovementTypeErrorCodes NotFound = new(10_008);
}

public static class Errors
{
    public static ErrorModel FinancialMovementTypeNotFound() => new(FinancialMovementTypeErrorCodes.NotFound, "FinancialMovementType Not Found.");
}
