using Andor.Foundation.Contracts.Results;

namespace Andor.Onboarding.Contracts.Responses;

internal sealed record SignupErrorCodes : ApplicationErrorCode
{
    private SignupErrorCodes(int original) : base(original)
    {
    }

    public static readonly SignupErrorCodes NotFound = new(13_000);
    public static readonly SignupErrorCodes InvalidCode = new(13_001);
    public static readonly SignupErrorCodes CodeExpired = new(13_002);
    public static readonly SignupErrorCodes AlreadyVerified = new(13_003);
}

public record SignupErrors
{
    public static ErrorModel SignupNotFound() => new(SignupErrorCodes.NotFound, "Signup request not found.");
    public static ErrorModel InvalidCode() => new(SignupErrorCodes.InvalidCode, "The verification code is invalid.");
    public static ErrorModel CodeExpired() => new(SignupErrorCodes.CodeExpired, "The verification code has expired.");
    public static ErrorModel AlreadyVerified() => new(SignupErrorCodes.AlreadyVerified, "This signup request was already verified.");
}
