using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Entities.Communications.Repositories;

public interface ICommandsPermissionRepository : ICommandRepository<Permission, PermissionId>
{
}
