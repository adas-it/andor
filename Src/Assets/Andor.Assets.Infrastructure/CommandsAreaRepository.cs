using Andor.Assets.Domain.Investments.Areas;
using Andor.Assets.Domain.Investments.Areas.Repositories;
using Andor.Assets.Domain.Investments.Areas.ValueObjects;

namespace Andor.Assets.Infrastructure;

public class CommandsAreaRepository : ICommandsAreaRepository
{
    private Dictionary<AreaId, Area> _areas = new Dictionary<AreaId, Area>();
    public async Task PersistAsync(Area entity, CancellationToken cancellationToken)
    {
        _areas[entity.Id] = entity;
        await Task.CompletedTask;
    }

    public async Task<Area?> GetByIdAsync(AreaId id, CancellationToken cancellationToken)
    {
        _areas.TryGetValue(id, out var area);
        return await Task.FromResult(area);
    }
}
