namespace Family.Budget.Application.Administrations.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Configurations.Errors;
using Family.Budget.Application.Dto.Configurations.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin.Repository;
using Mapster;
using MediatR;

public class GetByIdConfigurationQuery : IRequest<ConfigurationOutput>
{
    public Guid Id { get; private set; }
    public GetByIdConfigurationQuery(Guid Id)
    {
        this.Id = Id;
    }
}

public class GetByIdConfigurationQueryHandler : BaseCommands, IRequestHandler<GetByIdConfigurationQuery, ConfigurationOutput>
{
    public IConfigurationRepository _repository;
    public IFeatureFlagService _featureFlag;

    public GetByIdConfigurationQueryHandler(IConfigurationRepository repository,
        IFeatureFlagService featureFlag,
        Notifier notifier) : base(notifier)
    {
        _repository = repository;
        _featureFlag = featureFlag;
    }

    public async Task<ConfigurationOutput> Handle(GetByIdConfigurationQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(ConfigurationErrors.ConfigurationNotFound());

            return null!;
        }

        return item.Adapt<ConfigurationOutput>();
    }
}