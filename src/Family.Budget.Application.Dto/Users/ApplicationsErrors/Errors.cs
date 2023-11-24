namespace Family.Budget.Application.Dto.Users.Errors;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
public sealed record UserErrorCodes : ErrorCode
{
    private UserErrorCodes(int original) : base(original)
    {
    }

    public static readonly UserErrorCodes NotFound = new(20_000);
    public static readonly UserErrorCodes WrongCode = new(20_001);
    public static readonly UserErrorCodes EmailInUse = new(20_002);
}

public static class Errors
{
    public static ErrorModel UserNotFound() => new(UserErrorCodes.NotFound, "User Not Found.");
    public static ErrorModel WrongCode() => new(UserErrorCodes.WrongCode, "Wrong Code.");
    public static ErrorModel EmailInUse() => new(UserErrorCodes.EmailInUse, "Email In Use.");
}
