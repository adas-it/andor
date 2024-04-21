using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Administrations.Configurations;

namespace Andor.Application.Administrations.Configurations.Services;

public interface IConfigurationServices
{
    Task<ApplicationResult<ConfigurationOutput>> Handle(Configuration entity, CancellationToken cancellationToken);
}
