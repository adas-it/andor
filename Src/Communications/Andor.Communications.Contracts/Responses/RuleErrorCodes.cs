using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Contracts.Responses;

internal sealed record RuleErrorCodes : ApplicationErrorCode
{
    private RuleErrorCodes(int original) : base(original)
    {
    }

    public static readonly RuleErrorCodes NotFound = new(12_000);
    public static readonly RuleErrorCodes RuleValidation = new(12_001);
    public static readonly RuleErrorCodes ActionNotAllowed = new(12_002);
    public static readonly RuleErrorCodes NotificationNotSent = new(12_003);
}

public record RuleErrors
{
    public static ErrorModel RuleNotFound() => new(RuleErrorCodes.NotFound, "Rule Not Found.");
    public static ErrorModel RuleValidation() => new(RuleErrorCodes.RuleValidation, "Rule Validation.");
    public static ErrorModel ActionNotAllowed() => new(RuleErrorCodes.ActionNotAllowed, "You do not have permission to do this action.");
    public static ErrorModel NotificationNotSent() => new(RuleErrorCodes.NotificationNotSent, "Unfortunately the notification could not be sent.");
}
