using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Communications.Repositories;

public interface ICommandsPermissionRepository : ICommandRepository<Permission, PermissionId>
{
}
