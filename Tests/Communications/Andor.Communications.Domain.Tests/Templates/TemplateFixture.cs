using Andor.Communications.Domain.Tests.Rules;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.Tests.Templates;

/// <summary>
/// Fixture for creating Template instances for testing purposes.
/// </summary>
internal static class TemplateFixture
{
    public static async Task<Rule> CreateOwnerRuleAsync()
    {
        var (_, rule) = await RuleFixture.CreateValidRuleAsync();
        return rule!;
    }

    public static (DomainResult result, Template? template) CreateValidTemplate(
        Rule rule,
        string? value = null,
        string? contentLanguage = null,
        string? title = null,
        Partner? partner = null)
    {
        return Template.New(
            value ?? "Some template content",
            contentLanguage ?? "en-US",
            title ?? "Valid Title",
            partner ?? Partner.InHouse,
            rule);
    }
}
