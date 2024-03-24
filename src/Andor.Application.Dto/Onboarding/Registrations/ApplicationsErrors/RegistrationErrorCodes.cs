using Andor.Application.Dto.Common.ApplicationsErrors.Models;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Andor.Application.Dto.Common.ApplicationsErrors;
public sealed record RegistrationErrorCodes : ApplicationErrorCode
{
    private RegistrationErrorCodes(int original) : base(original)
    {
    }

    public static readonly RegistrationErrorCodes Validation = new(19_000);
    public static readonly RegistrationErrorCodes NotFound = new(19_001);
    public static readonly RegistrationErrorCodes DuplicateRegistration = new(19_002);
    public static readonly RegistrationErrorCodes EmailInUse = new(19_003);
    public static readonly RegistrationErrorCodes WrongCode = new(19_004);
}

public partial record Errors
{
    public static ErrorModel RegistrationValidation() => new(RegistrationErrorCodes.Validation,
        "Validation.");
    public static ErrorModel RegistrationNotFound() => new(RegistrationErrorCodes.NotFound,
        "Registration Not Found.");
    public static ErrorModel DuplicateRegistration() => new(RegistrationErrorCodes.DuplicateRegistration,
        "Duplicate Registration.");
    public static ErrorModel EmailInUse() => new(RegistrationErrorCodes.EmailInUse,
        "EmailInUse.");
    public static ErrorModel WrongCode() => new(RegistrationErrorCodes.WrongCode,
        "Wrong Code.");
}
