using Andor.Domain.Common.ValuesObjects;

namespace Andor.Onboarding.Domain.Errors;

public record SignupErrorCodes
{
    public static readonly DomainErrorCode SignupNotFound = DomainErrorCode.New(8_000);
    public static readonly DomainErrorCode InvalidCode = DomainErrorCode.New(8_001);
    public static readonly DomainErrorCode CodeExpired = DomainErrorCode.New(8_002);
    public static readonly DomainErrorCode AlreadyVerified = DomainErrorCode.New(8_003);

    // Info codes
    public static readonly DomainErrorCode SkippedValidations = DomainErrorCode.New(8_004);
}
