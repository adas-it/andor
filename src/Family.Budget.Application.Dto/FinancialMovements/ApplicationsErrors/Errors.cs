using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

namespace Family.Budget.Application.Dto.FinancialMovements.ApplicationsErrors;

public sealed record FinancialMovementErrorCodes : ErrorCode
{
    private FinancialMovementErrorCodes(int original) : base(original)
    {
    }

    public static readonly FinancialMovementErrorCodes NotFound = new(17_000);
}

public static class Errors
{
    public static ErrorModel FinancialMovementNotFound() => new(FinancialMovementErrorCodes.NotFound, "FinancialMovement Not Found.");
}
