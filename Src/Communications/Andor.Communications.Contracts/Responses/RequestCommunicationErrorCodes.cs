using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.Contracts.Responses;

internal sealed record RequestCommunicationErrorCodes : ApplicationErrorCode
{
    private RequestCommunicationErrorCodes(int original) : base(original)
    {
    }

    public static readonly RequestCommunicationErrorCodes RecipientNotFound = new(13_000);
    public static readonly RequestCommunicationErrorCodes RecipientInactive = new(13_001);
    public static readonly RequestCommunicationErrorCodes MarketingConsentRequired = new(13_002);
    public static readonly RequestCommunicationErrorCodes MissingRecipient = new(13_003);
}

public record RequestCommunicationErrors
{
    public static ErrorModel RecipientNotFound() => new(RequestCommunicationErrorCodes.RecipientNotFound,
        "No recipient is known for that user id yet — it may not have synced from Users.Service yet, retry shortly.");
    public static ErrorModel RecipientInactive() => new(RequestCommunicationErrorCodes.RecipientInactive,
        "This recipient's account is not active.");
    public static ErrorModel MarketingConsentRequired() => new(RequestCommunicationErrorCodes.MarketingConsentRequired,
        "This recipient has not opted into marketing communications.");
    public static ErrorModel MissingRecipient() => new(RequestCommunicationErrorCodes.MissingRecipient,
        "Provide either UserId or both RecipientEmail and ContentLanguage.");
}
