using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Assets.Domain.Investments.Areas.Repositories;

public interface ICommandsAreaRepository :
    ICommandRepository<Area, AreaId>
{
}
