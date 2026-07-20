namespace Andor.Communications.Contracts.Requests;

public record SendNotificationInput(Guid RuleId, string RecipientEmail, string Subject,
    string TemplateTitle, Dictionary<string, string> Values);