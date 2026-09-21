namespace Andor.Communications.Contracts.Requests;

// RecipientEmail stays required for the email/InHouse partner; RecipientId is what a Push-partner
// template needs instead - null for requests that never resolved a Recipient (e.g. the
// verification-code email, sent before a User exists).
public record SendNotificationInput(Guid RuleId, string RecipientEmail,
    string TemplateTitle, string ContentLanguage, Dictionary<string, string> Values, Guid? RecipientId = null);