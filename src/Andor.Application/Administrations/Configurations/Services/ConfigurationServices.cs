using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Administrations.Configurations;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Administrations.Configurations.ValueObjects;
using Andor.Domain.Onboarding.Registrations.Repositories.Models;

namespace Andor.Application.Administrations.Configurations.Services;
public class ConfigurationServices(IQueriesConfigurationRepository configurationRepository)
    : IConfigurationServices
{
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(Configuration entity, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        var listWithSameName = await configurationRepository.GetByNameAndStatusAsync(
            new SearchConfigurationInput(entity.Name, [ConfigurationState.Active, ConfigurationState.Awaiting]),
            cancellationToken);

        if (listWithSameName is not null && listWithSameName.Exists(x => x.Id != entity.Id))
        {
            if (listWithSameName.Exists(x => x.StartDate <= entity.StartDate && x.ExpireDate >= entity.StartDate && x.Id != entity.Id))
            {
                response.AddError(Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Exists(x => x.StartDate <= entity.ExpireDate && x.ExpireDate >= entity.ExpireDate && x.Id != entity.Id))
            {
                response.AddError(Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate());
            }
        }

        return response;
    }
}
