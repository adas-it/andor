namespace Family.Budget.Application.Dto.Accounts.Errors;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
public sealed record AccountErrorCodes : ErrorCode
{
    private AccountErrorCodes(int original) : base(original)
    {
    }

    public static readonly AccountErrorCodes NotFound = new(22_000);
    public static readonly AccountErrorCodes UserAlreadAddedToAccount = new(22_001);
}

public static class AccountError
{
    public static ErrorModel AccountNotFound() => new(AccountErrorCodes.NotFound, "Account not found.");
    public static ErrorModel UserAlreadAddedToAccount() => new(AccountErrorCodes.UserAlreadAddedToAccount, "User Alread Added To Account.");
}
