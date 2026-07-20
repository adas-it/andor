using Andor.Communications.Contracts.Responses;
using Andor.Communications.Domain;

namespace Andor.Communications.Application;

internal static class RuleMapperExtensions
{
    public static RuleOutput? ToRuleOutput(this Rule? rule)
    {
        if (rule == null) return null;

        return new RuleOutput(rule.Id, rule.Name,
            new KeyValuePair<int, string>(rule.Type.Key, rule.Type.Name), rule.CreatedAt);
    }
}
