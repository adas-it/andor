namespace Family.Budget.Application.Administrations.Queries;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Configurations.Responses;
using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Application.Models.FeatureFlag;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Mapster;
using MediatR;

public record ListConfigurationsQuery
    : PaginatedListInput, IRequest<ListConfigurationsOutput>
{
    public ListConfigurationsQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListConfigurationsQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }
}

public class ListConfigurationsQueryHandler : IRequestHandler<ListConfigurationsQuery, ListConfigurationsOutput>
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly IFeatureFlagService _featureFlag;

    public ListConfigurationsQueryHandler(IConfigurationRepository configurationRepository,
        IFeatureFlagService featureFlag)
    {
        _featureFlag = featureFlag;
        _configurationRepository = configurationRepository;
    }

    public async Task<ListConfigurationsOutput> Handle(
        ListConfigurationsQuery request,
        CancellationToken cancellationToken)
    {
        if(await _featureFlag.IsEnabledAsync(CurrentFeatures.FeatureFlagToTest) is false)
        {
            return new ListConfigurationsOutput(0,10,100, new List<ConfigurationOutput>());
        }

        var searchOutput = await _configurationRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (SearchOrder)request.Dir
            ),
            cancellationToken
        );

        return searchOutput.Adapt<ListConfigurationsOutput>();
    }
}