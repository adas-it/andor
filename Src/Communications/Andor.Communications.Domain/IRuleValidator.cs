using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.Validation;

namespace Andor.Communications.Domain;

public interface IRuleValidator : IDefaultValidator<Rule, RuleId>
{
}
