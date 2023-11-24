using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

namespace Family.Budget.Application.Dto.Currencies.Errors;

public sealed record CurrencyErrorCodes : ErrorCode
{
    private CurrencyErrorCodes(int original) : base(original)
    {
    }

    public static readonly CurrencyErrorCodes NotFound = new(14_000);
}

public static class Errors
{
    public static ErrorModel CurrencyNotFound() => new(CurrencyErrorCodes.NotFound, "Currency Not Found.");
}
