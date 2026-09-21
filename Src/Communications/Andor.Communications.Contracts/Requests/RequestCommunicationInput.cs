namespace Andor.Communications.Contracts.Requests;

// UserId enriches the request from the Recipient projection (Email/PreferredLanguage/"<name>")
// and gates Marketing sends on consent; RecipientEmail is for callers with no User yet (e.g. the
// signup verification code, sent before a User exists). At least one of the two must be set.
public record RequestCommunicationInput(
    Guid RuleId,
    string TemplateTitle,
    Guid? UserId,
    string? RecipientEmail,
    string? ContentLanguage,
    Dictionary<string, string>? Values);
