using Andor.Domain.Common.ValuesObjects;

namespace Users.Users.Errors;

public static class UserErrorMessages
{
    public const string EmailAlreadyInUse = "A user with this e-mail address already exists.";
}

public sealed record UserErrorCode
{
    public static readonly DomainErrorCode EmailAlreadyInUse = DomainErrorCode.New(4_001);
}
