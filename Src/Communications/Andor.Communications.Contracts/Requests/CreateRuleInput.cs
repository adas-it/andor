namespace Andor.Communications.Contracts.Requests;

public record RuleTemplateInput(string Value, string ContentLanguage, string Title, int Partner);

public record CreateRuleInput(string Name, int Type, List<RuleTemplateInput> Templates, bool Force);