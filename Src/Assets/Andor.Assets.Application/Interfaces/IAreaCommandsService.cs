using Andor.Assets.Application.Commands;
using Andor.Assets.Contracts;
using Andor.Foundation.Contracts.Results;

namespace Andor.Assets.Application.Interfaces;

public interface IAreaCommandsService
{
    Task<ApplicationResult<AreaOutput?>> CreateAreaAsync(CreateAreaCommand command);
}
