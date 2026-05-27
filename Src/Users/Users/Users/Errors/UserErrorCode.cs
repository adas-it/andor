using Andor.Domain.Common.ValuesObjects;

namespace Users.Users.Errors;

public static class UserErrorMessages
{
    public const string EmailAlreadyInUse = "A user with this e-mail address already exists.";
}

public sealed record UserErrorCode(int original) : DomainErrorCode(original)
{
    public static readonly DomainErrorCode EmailAlreadyInUse = new UserErrorCode(4_001);
}
