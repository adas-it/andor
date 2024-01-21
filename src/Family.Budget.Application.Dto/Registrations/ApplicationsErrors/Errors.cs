namespace Family.Budget.Application.Dto.Registrations.Errors;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

public sealed record RegistrationErrorCodes : ErrorCode
{
    private RegistrationErrorCodes(int original) : base(original)
    {
    }

    public static readonly RegistrationErrorCodes NotFound = new(19_000);
    public static readonly RegistrationErrorCodes DuplicateRegistration = new(19_001);
    public static readonly RegistrationErrorCodes EmailInUse = new(19_002);
    public static readonly RegistrationErrorCodes WrongCode = new(19_003);
}

public static class Errors
{
    public static ErrorModel RegistrationNotFound() => new(RegistrationErrorCodes.NotFound, 
        "Registration Not Found.");
    public static ErrorModel DuplicateRegistration() => new(RegistrationErrorCodes.DuplicateRegistration,
        "Duplicate Registration.");
    public static ErrorModel EmailInUse() => new(RegistrationErrorCodes.EmailInUse,
        "EmailInUse.");
    public static ErrorModel WrongCode() => new(RegistrationErrorCodes.WrongCode,
        "Wrong Code.");
}
