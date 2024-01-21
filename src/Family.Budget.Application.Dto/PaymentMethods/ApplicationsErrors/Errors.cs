using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

namespace Family.Budget.Application.Dto.PaymentMethods.Errors;

public sealed record PaymentMethodErrorCodes : ErrorCode
{
    private PaymentMethodErrorCodes(int original) : base(original)
    {
    }

    public static readonly PaymentMethodErrorCodes NotFound = new(15_000);
}

public static class Errors
{
    public static ErrorModel PaymentMethodNotFound() => new(PaymentMethodErrorCodes.NotFound, "PaymentMethod Not Found.");
}
