using Andor.Domain.Entities.Communications;
using Andor.Domain.Entities.Communications.Repositories;
using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Communication.Repositories;

public class CommandsRuleRepository(PrincipalContext context) :
    CommandsBaseRepository<Rule, RuleId>(context),
    ICommandsRuleRepository
{
}
