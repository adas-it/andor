using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Communications.Repositories;

public interface IQueriesPermissionRepository :
    IResearchableRepository<Permission, PermissionId, SearchInput>
{
}
