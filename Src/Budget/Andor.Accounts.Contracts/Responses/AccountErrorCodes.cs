using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Contracts.Responses;

internal sealed record AccountErrorCodes : ApplicationErrorCode
{
    private AccountErrorCodes(int original) : base(original)
    {
    }

    public static readonly AccountErrorCodes NotFound = new(14_000);
    public static readonly AccountErrorCodes AccountValidation = new(14_001);
    public static readonly AccountErrorCodes CurrencyNotFound = new(14_002);
}

public record AccountErrors
{
    public static ErrorModel AccountNotFound() => new(AccountErrorCodes.NotFound, "Account Not Found.");
    public static ErrorModel AccountValidation() => new(AccountErrorCodes.AccountValidation, "Account Validation.");
    public static ErrorModel CurrencyNotFound() => new(AccountErrorCodes.CurrencyNotFound, "Currency Not Found.");
}
