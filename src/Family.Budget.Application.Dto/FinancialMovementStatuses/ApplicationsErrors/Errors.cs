namespace Family.Budget.Application.Dto.MovementStatuses.ApplicationsErrors;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

public sealed record FinancialMovementStatusErrorCodes : ErrorCode
{
    private FinancialMovementStatusErrorCodes(int original) : base(original)
    {
    }

    public static readonly FinancialMovementStatusErrorCodes NotFound = new(10_007);
}

public static class Errors
{
    public static ErrorModel FinancialMovementStatusNotFound() => new(FinancialMovementStatusErrorCodes.NotFound, "FinancialMovementStatus Not Found.");
}
