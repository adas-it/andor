using Andor.Domain.Entities.Communications;
using Andor.Domain.Entities.Communications.Repositories;
using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Communication.Repositories;

public class CommandsPermissionRepository(PrincipalContext context) :
    CommandsBaseRepository<Permission, PermissionId>(context),
    ICommandsPermissionRepository
{
}
