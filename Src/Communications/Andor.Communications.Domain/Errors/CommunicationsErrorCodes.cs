using Andor.Domain.Common.ValuesObjects;

namespace Andor.Communications.Domain.Errors;

public record CommunicationsErrorCodes
{
    public static readonly DomainErrorCode ActionNotAllowed = DomainErrorCode.New(5_000);
    public static readonly DomainErrorCode RuleNotFound = DomainErrorCode.New(5_001);
    public static readonly DomainErrorCode RuleValidation = DomainErrorCode.New(5_003);

    // Info codes
    public static readonly DomainErrorCode SkippedValidations = DomainErrorCode.New(5_002);
}
