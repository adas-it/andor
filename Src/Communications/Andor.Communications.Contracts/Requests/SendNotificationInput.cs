namespace Andor.Communications.Contracts.Requests;

public record SendNotificationInput(Guid RuleId, string RecipientEmail,
    string TemplateTitle, string ContentLanguage, Dictionary<string, string> Values);