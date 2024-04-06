using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Entities.Admin.Configurations.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Administrations.Configurations.Queries;

public record GetConfigurationByIdQuery(ConfigurationId Id) : IRequest<ConfigurationOutput?>;

public class GetByIdConfigurationQueryHandler(IQueriesConfigurationRepository repository)
    : IRequestHandler<GetConfigurationByIdQuery, ConfigurationOutput?>
{
    public async Task<ConfigurationOutput?> Handle(GetConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);

        return item.Adapt<ConfigurationOutput?>();
    }
}
