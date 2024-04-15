using Andor.Domain.Communications;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Communication.Repositories;

public class CommandsRuleRepository(PrincipalContext context) :
    CommandsBaseRepository<Rule, RuleId>(context),
    ICommandsRuleRepository
{
}
