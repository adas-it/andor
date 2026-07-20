using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Communications.Domain.Repositories;

public interface ICommandsRuleRepository :
    ICommandRepository<Rule, RuleId>
{
}
