using Andor.Domain.Communications.Repositories;
using Andor.Domain.Communications.Users;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Communication.Repositories.Users;

public class CommandsRecipientRepository(PrincipalContext context) :
    CommandsBaseRepository<Recipient, RecipientId>(context),
    ICommandsRecipientRepository
{
}
